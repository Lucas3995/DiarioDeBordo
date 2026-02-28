using DiarioDeBordo.Application.Auth;
using DiarioDeBordo.Application.Auth.Login;
using DiarioDeBordo.Domain.Auth;
using DiarioDeBordo.Infrastructure.Auth;
using DiarioDeBordo.Persistence;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace DiarioDeBordo.Tests.Integration.Auth;

/// <summary>
/// Testes de integração do endpoint POST /api/auth/login.
/// Verifica autenticação, validação e formato do token retornado.
/// </summary>
public sealed class AuthControllerTests : IClassFixture<AuthControllerTestFactory>, IAsyncLifetime
{
    private readonly AuthControllerTestFactory _factory;
    private readonly HttpClient _client;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public AuthControllerTests(AuthControllerTestFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    public async Task InitializeAsync() => await _factory.SeedAdminAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    private static StringContent JsonBody(object obj) =>
        new(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");

    // ────────────────── Credenciais corretas → 200 + token ──────────────────

    [Fact]
    public async Task POST_login_CredenciaisCorretas_Retorna200ComTokenNaoVazio()
    {
        var response = await _client.PostAsync(
            "/api/auth/login",
            JsonBody(new { login = "admin", senha = "camaradinha@123" }));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<LoginResponse>(json, JsonOpts);

        result.Should().NotBeNull();
        result!.Sucesso.Should().BeTrue();
        result.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task POST_login_CredenciaisCorretas_TokenRetornadoEhJwtValido()
    {
        var response = await _client.PostAsync(
            "/api/auth/login",
            JsonBody(new { login = "admin", senha = "camaradinha@123" }));

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<LoginResponse>(json, JsonOpts);

        // JWT válido tem exatamente 3 partes separadas por '.'
        result!.Token!.Split('.').Should().HaveCount(3);
    }

    [Fact]
    public async Task POST_login_CredenciaisCorretas_ExpiresAtNoFuturo()
    {
        var response = await _client.PostAsync(
            "/api/auth/login",
            JsonBody(new { login = "admin", senha = "camaradinha@123" }));

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<LoginResponse>(json, JsonOpts);

        result!.ExpiresAt.Should().NotBeNull();
        result.ExpiresAt!.Value.Should().BeAfter(DateTime.UtcNow);
    }

    // ────────────────── Credenciais erradas → 401 ──────────────────

    [Fact]
    public async Task POST_login_SenhaErrada_Retorna401()
    {
        var response = await _client.PostAsync(
            "/api/auth/login",
            JsonBody(new { login = "admin", senha = "senhaErrada" }));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task POST_login_LoginInexistente_Retorna401()
    {
        var response = await _client.PostAsync(
            "/api/auth/login",
            JsonBody(new { login = "naoexiste", senha = "qualquer" }));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // ────────────────── Validação → 400 ──────────────────

    [Fact]
    public async Task POST_login_LoginVazio_Retorna400()
    {
        var response = await _client.PostAsync(
            "/api/auth/login",
            JsonBody(new { login = "", senha = "camaradinha@123" }));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task POST_login_SenhaVazia_Retorna400()
    {
        var response = await _client.PostAsync(
            "/api/auth/login",
            JsonBody(new { login = "admin", senha = "" }));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

/// <summary>
/// Factory para testes do AuthController: usa InMemory DB com seed do admin.
/// O ambiente "Testing" pula Persistence no Program.cs, por isso registramos manualmente aqui.
/// </summary>
public sealed class AuthControllerTestFactory : WebApplicationFactory<Program>
{
    private const string JwtKey = "chave-de-teste-para-auth-controller-abc-xyz";
    private const string JwtIssuer = "diariodebordo-auth-test";
    private const string JwtAudience = "diariodebordo-clients-auth-test";
    private readonly string _dbName = $"TestAuthController_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Key", JwtKey },
                { "Jwt:Issuer", JwtIssuer },
                { "Jwt:Audience", JwtAudience }
            });
        });

        builder.ConfigureServices(services =>
        {
            // Remover DbContext existente (se houver) e registrar InMemory
            var dbDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<DiarioDeBordoDbContext>));
            if (dbDescriptor is not null)
                services.Remove(dbDescriptor);

            services.AddDbContext<DiarioDeBordoDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));

            // Repositório de usuário
            var repoDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IUsuarioRepository));
            if (repoDescriptor is null)
                services.AddScoped<IUsuarioRepository, DiarioDeBordo.Persistence.Auth.UsuarioRepository>();

            // IPasswordHasher pode já estar registrado via AddInfrastructure — não duplicar
            if (!services.Any(d => d.ServiceType == typeof(IPasswordHasher)))
                services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
        });
    }

    public async Task SeedAdminAsync()
    {
        using var scope = Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<DiarioDeBordoDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        if (!await ctx.Usuarios.AnyAsync())
        {
            var admin = new Usuario("admin", Perfil.Admin);
            admin.DefinirSenhaHash(hasher.Hash("camaradinha@123"));
            ctx.Usuarios.Add(admin);
            await ctx.SaveChangesAsync();
        }
    }
}
