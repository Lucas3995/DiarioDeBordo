using DiarioDeBordo.Core.FeatureFlags;
using DiarioDeBordo.Core.Primitivos;
using Xunit;
using DiarioDeBordo.Infrastructure;
using DiarioDeBordo.Infrastructure.Persistencia;
using DiarioDeBordo.Module.Acervo.Commands;
using DiarioDeBordo.Module.Acervo.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;

namespace DiarioDeBordo.Tests.E2E;

/// <summary>
/// Walking Skeleton E2E test — proves the full architectural stack works end-to-end.
///
/// SC1: A content can be created with a title, saved to PostgreSQL, retrieved —
///      end-to-end — using the domain commands and queries.
/// SC2: The test uses the same MediatR handlers and domain entities as the running application,
///      with only the PostgreSQL connection string swapped (Testcontainers).
///
/// This test is the ultimate proof that the architecture works as designed.
/// It will run in CI on ubuntu-latest (Docker available) per the ci.yml workflow.
/// </summary>
public class WalkingSkeletonTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("diariodebordo_e2e")
        .WithUsername("e2e_user")
        .WithPassword("e2e_password_secure")
        .Build();

    private IServiceProvider _services = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        var services = new ServiceCollection();

        // Infrastructure: real PostgreSQL (Testcontainers) — registers DbContext, repos and query service
        services.AddInfrastructure(_postgres.GetConnectionString());

        // MediatR: all handlers from Module.Acervo
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CriarConteudoCommand).Assembly);
        });

        // Feature flags placeholder
        services.AddSingleton<IFeatureFlags, FeatureFlagsPlaceholder>();

        services.AddLogging(l => l.SetMinimumLevel(LogLevel.Warning));

        _services = services.BuildServiceProvider();

        // Apply migrations
        using var scope = _services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<DiarioDeBordoDbContext>();
        await ctx.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
    }

    /// <summary>
    /// SC1: Create content → persist → retrieve → appears in list.
    /// This is the walking skeleton proof.
    /// </summary>
    [Fact]
    public async Task WalkingSkeleton_CriarConteudo_Aparece_NaListagem()
    {
        using var scope = _services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var usuarioId = Guid.NewGuid();
        const string titulo = "Dune — Frank Herbert";

        // Step 1: Create content via command
        var criarResult = await mediator.Send(
            new CriarConteudoCommand(usuarioId, titulo));

        Assert.True(criarResult.IsSuccess, $"CriarConteudo falhou: {criarResult.Error?.Mensagem}");
        var conteudoId = criarResult.Value;
        Assert.NotEqual(Guid.Empty, conteudoId);

        // Step 2: Retrieve via list query
        var listarResult = await mediator.Send(
            new ListarConteudosQuery(usuarioId, PaginacaoParams.Padrao));

        Assert.True(listarResult.IsSuccess);
        Assert.Equal(1, listarResult.Value!.TotalItems);
        Assert.Single(listarResult.Value.Items);
        Assert.Equal(titulo, listarResult.Value.Items[0].Titulo);
    }

    /// <summary>SC2: User isolation — user B cannot see user A's content.</summary>
    [Fact]
    public async Task WalkingSkeleton_IsolamentoPorUsuario_UsuarioBNaoVeConteudoDeA()
    {
        using var scope = _services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var usuarioA = Guid.NewGuid();
        var usuarioB = Guid.NewGuid();

        // User A creates content
        await mediator.Send(new CriarConteudoCommand(usuarioA, "Conteúdo Privado de A"));

        // User B queries — should see 0 items
        var resultado = await mediator.Send(
            new ListarConteudosQuery(usuarioB, PaginacaoParams.Padrao));

        Assert.True(resultado.IsSuccess);
        Assert.Equal(0, resultado.Value!.TotalItems);
    }

    /// <summary>SC1 extended: Multiple contents — list is paginated correctly.</summary>
    [Fact]
    public async Task WalkingSkeleton_MultiploConteudos_ListagemPaginada()
    {
        using var scope = _services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var usuarioId = Guid.NewGuid();

        // Create 3 contents
        for (var i = 1; i <= 3; i++)
        {
            var result = await mediator.Send(
                new CriarConteudoCommand(usuarioId, $"Conteúdo {i}"));
            Assert.True(result.IsSuccess);
        }

        // List with small page size
        var lista = await mediator.Send(
            new ListarConteudosQuery(usuarioId, new PaginacaoParams(1, 2)));

        Assert.True(lista.IsSuccess);
        Assert.Equal(3, lista.Value!.TotalItems);
        Assert.Equal(2, lista.Value.Items.Count); // page 1 has 2 items
        Assert.Equal(2, lista.Value.TotalPaginas); // ceil(3/2) = 2
        Assert.True(lista.Value.TemProximaPagina);
    }

    /// <summary>I-01 via E2E: title validation propagates from handler to caller.</summary>
    [Fact]
    public async Task WalkingSkeleton_TituloVazio_RetornaErro_NaoSalva()
    {
        using var scope = _services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var usuarioId = Guid.NewGuid();

        var resultado = await mediator.Send(new CriarConteudoCommand(usuarioId, ""));

        Assert.False(resultado.IsSuccess);
        Assert.Equal("TITULO_OBRIGATORIO", resultado.Error!.Codigo);

        // Verify nothing was saved
        var lista = await mediator.Send(
            new ListarConteudosQuery(usuarioId, PaginacaoParams.Padrao));
        Assert.Equal(0, lista.Value!.TotalItems);
    }
}
