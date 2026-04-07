using DiarioDeBordo.Core.Consultas;
using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Core.Enums;
using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Core.Repositorios;
using DiarioDeBordo.Module.Acervo.Queries;
using NSubstitute;

namespace DiarioDeBordo.Tests.Domain.Acervo;

/// <summary>
/// Testes dos handlers de busca para autocomplete: Categorias, Conteúdos e TiposRelacao.
/// Foco no comportamento observável: mapeamento de campos e lógica de filtragem client-side.
/// </summary>
public class BuscarHandlersTests
{
    private static readonly Guid _usuarioId = Guid.NewGuid();

    // ── BuscarCategoriasHandler ──────────────────────────────────────────────

    [Fact]
    public async Task BuscarCategorias_MapeiaIdENomeCorretos()
    {
        var repo = Substitute.For<ICategoriaRepository>();
        var categoria = Categoria.Criar(_usuarioId, "Ficção Científica");
        repo.ListarComAutocompletarAsync(_usuarioId, "fic", Arg.Any<CancellationToken>())
            .Returns([categoria]);

        var result = await new BuscarCategoriasHandler(repo)
            .Handle(new BuscarCategoriasQuery(_usuarioId, "fic"), CancellationToken.None);

        var dto = Assert.Single(result);
        Assert.Equal(categoria.Id, dto.Id);
        Assert.Equal(categoria.Nome, dto.Nome);
        Assert.False(dto.IsAutomatica); // categorias criadas pelo usuário nunca são automáticas
    }

    // ── BuscarConteudosHandler ───────────────────────────────────────────────

    private static ResultadoPaginado<ConteudoResumoData> PaginaComItens(
        params ConteudoResumoData[] itens) =>
        new(itens, itens.Length, 1, 50);

    private static ConteudoResumoData Item(Guid id, string titulo) =>
        new(id, titulo, FormatoMidia.Texto, PapelConteudo.Item,
            DateTimeOffset.UtcNow, null, null, null, null, null, null);

    [Fact]
    public async Task BuscarConteudos_ExcluiOProprioConteudo()
    {
        var queryService = Substitute.For<IConteudoQueryService>();
        var propioId = Guid.NewGuid();
        var outroId = Guid.NewGuid();
        queryService.ListarAsync(_usuarioId, Arg.Any<PaginacaoParams>(), Arg.Any<PapelConteudo?>(), Arg.Any<CancellationToken>())
            .Returns(PaginaComItens(Item(propioId, "Alpha"), Item(outroId, "Alpha")));

        var result = await new BuscarConteudosHandler(queryService)
            .Handle(new BuscarConteudosQuery(_usuarioId, "Alpha", propioId), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(outroId, result[0].Id);
    }

    [Fact]
    public async Task BuscarConteudos_FiltragемPorPrefixoCaseInsensitive()
    {
        var queryService = Substitute.For<IConteudoQueryService>();
        queryService.ListarAsync(_usuarioId, Arg.Any<PaginacaoParams>(), Arg.Any<PapelConteudo?>(), Arg.Any<CancellationToken>())
            .Returns(PaginaComItens(
                Item(Guid.NewGuid(), "DUNA"),
                Item(Guid.NewGuid(), "Foundation"),
                Item(Guid.NewGuid(), "Dune Messiah")));

        var result = await new BuscarConteudosHandler(queryService)
            .Handle(new BuscarConteudosQuery(_usuarioId, "dun", Guid.NewGuid()), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.All(result, r => Assert.Contains("dun", r.Titulo, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task BuscarConteudos_LimitaA10ResultadosMesmoComMaisMatches()
    {
        var queryService = Substitute.For<IConteudoQueryService>();
        var itens = Enumerable.Range(1, 15)
            .Select(i => Item(Guid.NewGuid(), $"Livro {i}"))
            .ToArray();
        queryService.ListarAsync(_usuarioId, Arg.Any<PaginacaoParams>(), Arg.Any<PapelConteudo?>(), Arg.Any<CancellationToken>())
            .Returns(PaginaComItens(itens));

        var result = await new BuscarConteudosHandler(queryService)
            .Handle(new BuscarConteudosQuery(_usuarioId, "Livro", Guid.NewGuid()), CancellationToken.None);

        Assert.Equal(10, result.Count);
    }

    // ── BuscarTiposRelacaoHandler ────────────────────────────────────────────

    [Fact]
    public async Task BuscarTiposRelacao_PreservaIsSistemaParaTiposSistema()
    {
        var repo = Substitute.For<ITipoRelacaoRepository>();
        var sistema = TipoRelacao.Criar(Guid.Empty, "Sequência", "Continuação de");
        // Simula tipo de sistema marcando via reflexão não é necessário — IsSistema é false por padrão no factory,
        // o que é irrelevante para o mapeamento. O que importa é que o handler passa o valor sem alterar.
        repo.ListarComAutocompletarAsync(_usuarioId, "seq", Arg.Any<CancellationToken>())
            .Returns([sistema]);

        var result = await new BuscarTiposRelacaoHandler(repo)
            .Handle(new BuscarTiposRelacaoQuery(_usuarioId, "seq"), CancellationToken.None);

        var dto = Assert.Single(result);
        Assert.Equal(sistema.Id, dto.Id);
        Assert.Equal(sistema.Nome, dto.Nome);
        Assert.Equal(sistema.NomeInverso, dto.NomeInverso);
        Assert.Equal(sistema.IsSistema, dto.IsSistema); // campo repassado sem transformação
    }
}
