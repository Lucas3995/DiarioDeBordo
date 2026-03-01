using DiarioDeBordo.Application.Obras;
using DiarioDeBordo.Application.Obras.AtualizarPosicao;
using DiarioDeBordo.Application.Obras.Listar;
using DiarioDeBordo.Application.Obras.ObterPorIdOuNome;
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

    // --------------- GET por id (prévia) - relatório item 7 ---------------

    [Fact]
    public async Task GET_obras_PorId_ComToken_DeveRetornar200QuandoExistir()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _factory.GerarToken());
        var id = await _factory.ObterPrimeiroIdObraAsync();

        var resposta = await _client.GetAsync($"/api/obras/{id}");

        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await resposta.Content.ReadAsStringAsync();
        var obra = JsonSerializer.Deserialize<ObraDetalheDto>(json, JsonOpts);
        obra.Should().NotBeNull();
        obra!.Id.Should().Be(id);
    }

    [Fact]
    public async Task GET_obras_PorId_Inexistente_DeveRetornar404()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _factory.GerarToken());
        var id = Guid.NewGuid();

        var resposta = await _client.GetAsync($"/api/obras/{id}");

        resposta.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // --------------- PATCH posição - relatório item 7 ---------------

    [Fact]
    public async Task PATCH_posicao_ComIdExistente_DeveRetornar200EAtualizar()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _factory.GerarToken());
        var id = await _factory.ObterPrimeiroIdObraAsync();

        var body = new { idObra = id, nomeObra = (string?)null, novaPosicao = 999, dataUltimaAtualizacao = (DateTime?)null, criarSeNaoExistir = false, nomeParaCriar = (string?)null, tipoParaCriar = (string?)null, ordemPreferenciaParaCriar = (int?)null };
        var content = new StringContent(JsonSerializer.Serialize(body), System.Text.Encoding.UTF8, "application/json");

        var resposta = await _client.PatchAsync("/api/obras/posicao", content);

        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseJson = await resposta.Content.ReadAsStringAsync();
        var response = JsonSerializer.Deserialize<AtualizarPosicaoObraResponse>(responseJson, JsonOpts);
        response.Should().NotBeNull();
        response!.Id.Should().Be(id);
        response.Criada.Should().BeFalse();
    }

    // --------------- PATCH posição obra nova (Demanda 5: tipoParaCriar como string camelCase) ---------------

    /// <summary>
    /// Relatório Demanda 5 — item 1: API deve aceitar tipoParaCriar como string em camelCase (ex.: "manga").
    /// Sem JsonStringEnumConverter no Program.cs o binding falha e retorna 400.
    /// </summary>
    [Fact]
    public async Task PATCH_posicao_ObraNova_ComTipoParaCriarStringCamelCase_DeveRetornar200ECriadaTrue()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _factory.GerarToken());
        var nomeInexistente = "Obra Nova TDD " + Guid.NewGuid().ToString("N")[..8];
        var body = new
        {
            idObra = (Guid?)null,
            nomeObra = nomeInexistente,
            novaPosicao = 1,
            dataUltimaAtualizacao = (DateTime?)null,
            criarSeNaoExistir = true,
            nomeParaCriar = nomeInexistente,
            tipoParaCriar = "manga",
            ordemPreferenciaParaCriar = 0
        };
        var content = new StringContent(JsonSerializer.Serialize(body), System.Text.Encoding.UTF8, "application/json");

        var resposta = await _client.PatchAsync("/api/obras/posicao", content);

        resposta.StatusCode.Should().Be(HttpStatusCode.OK, "a API deve aceitar tipoParaCriar como string camelCase (JsonStringEnumConverter)");
        var responseJson = await resposta.Content.ReadAsStringAsync();
        var response = JsonSerializer.Deserialize<AtualizarPosicaoObraResponse>(responseJson, JsonOpts);
        response.Should().NotBeNull();
        response!.Criada.Should().BeTrue();
        response.Id.Should().NotBe(Guid.Empty);
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

            // Registrar repositórios usando o context InMemory
            var repoLeituraDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IObraLeituraRepository));
            if (repoLeituraDescriptor is null)
            {
                services.AddScoped<IObraLeituraRepository,
                    DiarioDeBordo.Persistence.Obras.ObraLeituraRepository>();
            }
            var repoEscritaDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IObraEscritaRepository));
            if (repoEscritaDescriptor is null)
            {
                services.AddScoped<IObraEscritaRepository,
                    DiarioDeBordo.Persistence.Obras.ObraEscritaRepository>();
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

    /// <summary>Retorna o Id da primeira obra (para testes que precisam de um id existente).</summary>
    public async Task<Guid> ObterPrimeiroIdObraAsync()
    {
        using var scope = Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<DiarioDeBordoDbContext>();
        var obra = await ctx.Obras.OrderBy(o => o.OrdemPreferencia).FirstAsync();
        return obra.Id;
    }
}
