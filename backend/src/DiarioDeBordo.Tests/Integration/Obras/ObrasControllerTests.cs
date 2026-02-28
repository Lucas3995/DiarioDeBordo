using DiarioDeBordo.Application.Obras.Listar;
using DiarioDeBordo.Domain.Obras;
using DiarioDeBordo.Persistence;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DiarioDeBordo.Tests.Integration.Obras;

/// <summary>
/// Testes de integração do endpoint GET /api/obras via WebApplicationFactory.
/// Substitui a persistência real por banco in-memory para isolamento.
/// Valida autenticação, paginação e estrutura da resposta.
/// </summary>
public sealed class ObrasControllerTests : IClassFixture<ObrasControllerTestFactory>, IAsyncLifetime
{
    private readonly ObrasControllerTestFactory _factory;
    private readonly HttpClient _client;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public ObrasControllerTests(ObrasControllerTestFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    public async Task InitializeAsync() => await _factory.PopularObrasAsync(3);

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GET_obras_SemToken_DeveRetornar401()
    {
        var resposta = await _client.GetAsync("/api/obras");

        resposta.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GET_obras_ComToken_DeveRetornar200()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _factory.GerarToken());

        var resposta = await _client.GetAsync("/api/obras?pageIndex=0&pageSize=10");

        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_obras_ComToken_DeveRetornarJsonComItemsETotalCount()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _factory.GerarToken());

        var resposta = await _client.GetAsync("/api/obras?pageIndex=0&pageSize=10");

        resposta.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await resposta.Content.ReadAsStringAsync();
        var resultado = JsonSerializer.Deserialize<ObrasAcompanhamentoListResponse>(json, JsonOpts);

        resultado.Should().NotBeNull();
        resultado!.TotalCount.Should().BeGreaterThanOrEqualTo(3);
        resultado.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GET_obras_PageSizeInvalido_DeveRetornar400()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _factory.GerarToken());

        var resposta = await _client.GetAsync("/api/obras?pageIndex=0&pageSize=7");

        resposta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GET_obras_DeveRetornarCamposEsperadosNoItem()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _factory.GerarToken());

        var resposta = await _client.GetAsync("/api/obras?pageIndex=0&pageSize=10");
        var json = await resposta.Content.ReadAsStringAsync();
        var resultado = JsonSerializer.Deserialize<ObrasAcompanhamentoListResponse>(json, JsonOpts);

        var item = resultado!.Items.First();
        item.Id.Should().NotBeNullOrEmpty();
        item.Nome.Should().NotBeNullOrEmpty();
        item.Tipo.Should().NotBeNullOrEmpty();
        item.PosicaoAtual.Should().BeGreaterThan(0);
    }
}

/// <summary>
/// Factory para testes do ObrasController: substitui o banco real por InMemory
/// e registra os demais serviços (incluindo IObraLeituraRepository via Persistence).
/// </summary>
public sealed class ObrasControllerTestFactory : WebApplicationFactory<Program>
{
    private const string JwtKey = "chave-de-teste-para-obras-controller-abc";
    private const string JwtIssuer = "diariodebordo-test";
    private const string JwtAudience = "diariodebordo-clients-test";
    private readonly string _dbName = $"TestObrasController_{Guid.NewGuid()}";

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
            // Remover o registro de DbContext existente e substituir por InMemory
            var dbDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<DiarioDeBordoDbContext>));
            if (dbDescriptor is not null)
                services.Remove(dbDescriptor);

            services.AddDbContext<DiarioDeBordoDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));

            // Registrar o repositório de leitura usando o context InMemory
            var repoDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IObraLeituraRepository));
            if (repoDescriptor is null)
            {
                services.AddScoped<IObraLeituraRepository,
                    DiarioDeBordo.Persistence.Obras.ObraLeituraRepository>();
            }
        });
    }

    public string GerarToken()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: JwtIssuer,
            audience: JwtAudience,
            claims: [new Claim(ClaimTypes.Name, "usuario-teste")],
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task PopularObrasAsync(int quantidade)
    {
        using var scope = Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<DiarioDeBordoDbContext>();

        if (!await ctx.Obras.AnyAsync())
        {
            for (var i = 1; i <= quantidade; i++)
            {
                ctx.Obras.Add(new Obra(
                    $"Obra de Teste {i}",
                    TipoObra.Manga,
                    posicaoAtual: i * 10,
                    dataUltimaAtualizacaoPosicao: new DateTime(2026, 1, i, 0, 0, 0, DateTimeKind.Utc),
                    ordemPreferencia: i));
            }

            await ctx.SaveChangesAsync();
        }
    }
}
