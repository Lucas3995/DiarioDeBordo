using DiarioDeBordo.Application.Obras.Listar;
using DiarioDeBordo.Domain.Common;
using DiarioDeBordo.Domain.Obras;
using DiarioDeBordo.Persistence;
using DiarioDeBordo.Persistence.Obras;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DiarioDeBordo.Tests.Integration.Obras;

/// <summary>
/// Testes de integração para GetObrasAcompanhamentoQueryHandler + ObraLeituraRepository.
/// Usam DbContext com provider InMemory para validar paginação, ordenação e projeção
/// sem depender de banco PostgreSQL real.
/// Padrão AAA; cada teste cria contexto isolado.
/// </summary>
public sealed class GetObrasAcompanhamentoTests
{
    private static readonly IClock _clock = Mock.Of<IClock>(c => c.UtcNow == new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc));

    private static DiarioDeBordoDbContext CriarContexto()
    {
        var options = new DbContextOptionsBuilder<DiarioDeBordoDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new DiarioDeBordoDbContext(options);
    }

    private static Obra CriarObra(
        string nome,
        TipoObra tipo,
        int posicao,
        int ordemPreferencia,
        ProximaInfoTipo? proximaInfoTipo = null,
        int? diasOuQuantidade = null)
    {
        var obra = new Obra(nome, tipo, posicao, DateTime.UtcNow, ordemPreferencia, _clock);
        if (proximaInfoTipo == ProximaInfoTipo.DiasAteProxima && diasOuQuantidade.HasValue)
            obra.DefinirDiasAteProxima(diasOuQuantidade.Value);
        else if (proximaInfoTipo == ProximaInfoTipo.PartesJaPublicadas && diasOuQuantidade.HasValue)
            obra.DefinirPartesJaPublicadas(diasOuQuantidade.Value);
        return obra;
    }

    // --- TotalCount ---

    [Fact]
    public async Task ListarPaginado_DeveRetornarTotalCountCorreto()
    {
        // Arrange
        await using var ctx = CriarContexto();
        ctx.Obras.AddRange(
            CriarObra("Obra 1", TipoObra.Manga, 10, 1),
            CriarObra("Obra 2", TipoObra.Anime, 5, 2),
            CriarObra("Obra 3", TipoObra.Manhwa, 20, 3));
        await ctx.SaveChangesAsync();
        var repo = new ObraLeituraRepository(ctx);

        // Act
        var (_, totalCount) = await repo.ListarPaginadoAsync(0, 10);

        // Assert
        totalCount.Should().Be(3);
    }

    // --- Paginação ---

    [Fact]
    public async Task ListarPaginado_ComPageSize2_DeveRetornar2Itens()
    {
        // Arrange
        await using var ctx = CriarContexto();
        for (int i = 1; i <= 5; i++)
            ctx.Obras.Add(CriarObra($"Obra {i}", TipoObra.Manga, i, i));
        await ctx.SaveChangesAsync();
        var repo = new ObraLeituraRepository(ctx);

        // Act
        var (itens, totalCount) = await repo.ListarPaginadoAsync(0, 2);

        // Assert
        itens.Should().HaveCount(2);
        totalCount.Should().Be(5);
    }

    [Fact]
    public async Task ListarPaginado_PaginaSeguinte_DeveRetornarSegundaFatia()
    {
        // Arrange
        await using var ctx = CriarContexto();
        for (int i = 1; i <= 5; i++)
            ctx.Obras.Add(CriarObra($"Obra {i}", TipoObra.Manga, i, i));
        await ctx.SaveChangesAsync();
        var repo = new ObraLeituraRepository(ctx);

        // Act
        var (pagina1, _) = await repo.ListarPaginadoAsync(0, 3);
        var (pagina2, _) = await repo.ListarPaginadoAsync(1, 3);

        // Assert
        pagina1.Should().HaveCount(3);
        pagina2.Should().HaveCount(2);
        pagina1.Select(o => o.Nome).Should().NotIntersectWith(pagina2.Select(o => o.Nome));
    }

    // --- Ordenação por OrdemPreferencia ---

    [Fact]
    public async Task ListarPaginado_DeveRetornarOrdenadasPorOrdemPreferencia()
    {
        // Arrange
        await using var ctx = CriarContexto();
        ctx.Obras.AddRange(
            CriarObra("Terceira", TipoObra.Manga, 1, ordemPreferencia: 3),
            CriarObra("Primeira", TipoObra.Anime, 2, ordemPreferencia: 1),
            CriarObra("Segunda", TipoObra.Manhwa, 3, ordemPreferencia: 2));
        await ctx.SaveChangesAsync();
        var repo = new ObraLeituraRepository(ctx);

        // Act
        var (itens, _) = await repo.ListarPaginadoAsync(0, 10);

        // Assert
        itens.Select(o => o.Nome)
            .Should().ContainInOrder("Primeira", "Segunda", "Terceira");
    }

    // --- Handler + Repository (integração completa sem EF mock) ---

    [Fact]
    public async Task Handler_ComRepositorioReal_DeveRetornarDtosCorretamente()
    {
        // Arrange
        await using var ctx = CriarContexto();
        var obra = CriarObra("Solo Leveling", TipoObra.Manhwa, 179, 1,
            ProximaInfoTipo.DiasAteProxima, 7);
        ctx.Obras.Add(obra);
        await ctx.SaveChangesAsync();

        var repo = new ObraLeituraRepository(ctx);
        var handler = new GetObrasAcompanhamentoQueryHandler(repo);
        var query = new GetObrasAcompanhamentoQuery(0, 10);

        // Act
        var resultado = await handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.TotalCount.Should().Be(1);
        var item = resultado.Items.Single();
        item.Nome.Should().Be("Solo Leveling");
        item.Tipo.Should().Be("manhwa");
        item.PosicaoAtual.Should().Be(179);
        item.ProximaInfo.Should().NotBeNull();
        item.ProximaInfo!.Tipo.Should().Be("dias_ate_proxima");
        item.ProximaInfo.Dias.Should().Be(7);
    }

    [Fact]
    public async Task Handler_ComListaVazia_DeveRetornarRespostaVazia()
    {
        // Arrange
        await using var ctx = CriarContexto();
        var repo = new ObraLeituraRepository(ctx);
        var handler = new GetObrasAcompanhamentoQueryHandler(repo);

        // Act
        var resultado = await handler.Handle(new GetObrasAcompanhamentoQuery(0, 10), CancellationToken.None);

        // Assert
        resultado.Items.Should().BeEmpty();
        resultado.TotalCount.Should().Be(0);
    }
}
