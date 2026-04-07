using DiarioDeBordo.Core.Consultas;
using DiarioDeBordo.Core.Enums;
using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Module.Acervo.DTOs;
using DiarioDeBordo.Module.Acervo.Queries;
using NSubstitute;

namespace DiarioDeBordo.Tests.Domain.Acervo;

/// <summary>
/// Testes dos handlers de listagem paginada: conteúdos e sessões.
/// Foco em: integridade da paginação e corretude do mapeamento de campos.
/// </summary>
public class ListarHandlersTests
{
    private static readonly Guid _usuarioId = Guid.NewGuid();

    // ── ListarConteudosHandler ───────────────────────────────────────────────

    [Fact]
    public async Task ListarConteudos_PreservaMetadadosDePaginacao()
    {
        var queryService = Substitute.For<IConteudoQueryService>();
        var data = new ResultadoPaginado<ConteudoResumoData>(
            [new(Guid.NewGuid(), "Duna", FormatoMidia.Texto, PapelConteudo.Item,
                 DateTimeOffset.UtcNow, Classificacao.Gostei, "Livro", null, null, null, null)],
            totalItems: 57,
            paginaAtual: 3,
            tamanhoPagina: 20);
        queryService.ListarAsync(_usuarioId, Arg.Any<PaginacaoParams>(), Arg.Any<PapelConteudo?>(), Arg.Any<CancellationToken>())
            .Returns(data);

        var result = await new ListarConteudosHandler(queryService)
            .Handle(new ListarConteudosQuery(_usuarioId, new PaginacaoParams(3, 20)), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(57, result.Value!.TotalItems);
        Assert.Equal(3, result.Value.PaginaAtual);
        Assert.Equal(20, result.Value.TamanhoPagina);
    }

    [Fact]
    public async Task ListarConteudos_NaoIncluidNotaNaVisaoDeListagem()
    {
        // Nota é omitida na listagem por performance — apenas no detalhe (comentário do handler)
        var queryService = Substitute.For<IConteudoQueryService>();
        queryService.ListarAsync(_usuarioId, Arg.Any<PaginacaoParams>(), Arg.Any<PapelConteudo?>(), Arg.Any<CancellationToken>())
            .Returns(new ResultadoPaginado<ConteudoResumoData>(
                [new(Guid.NewGuid(), "Fundação", FormatoMidia.Texto, PapelConteudo.Item,
                     DateTimeOffset.UtcNow, null, null, null, null, null, null)],
                1, 1, 20));

        var result = await new ListarConteudosHandler(queryService)
            .Handle(new ListarConteudosQuery(_usuarioId, PaginacaoParams.Padrao), CancellationToken.None);

        Assert.Null(Assert.Single(result.Value!.Items).Nota);
    }

    // ── ListarSessoesHandler ─────────────────────────────────────────────────

    [Fact]
    public async Task ListarSessoes_MapeiaTodosOsCamposDoDto()
    {
        var queryService = Substitute.For<IConteudoQueryService>();
        var paiId = Guid.NewGuid();
        var sessaoId = Guid.NewGuid();
        var criadoEm = DateTimeOffset.UtcNow.AddDays(-1);

        queryService.ListarSessoesAsync(paiId, _usuarioId, Arg.Any<PaginacaoParams>(), Arg.Any<CancellationToken>())
            .Returns(new ResultadoPaginado<SessaoData>(
                [new SessaoData(sessaoId, "Ep. 1", criadoEm, Classificacao.Gostei, 8.5m, "Ótimo episódio")],
                1, 1, 50));

        var result = await new ListarSessoesHandler(queryService)
            .Handle(new ListarSessoesQuery(paiId, _usuarioId, PaginacaoParams.Padrao), CancellationToken.None);

        Assert.True(result.IsSuccess);
        var dto = Assert.Single(result.Value!.Items);
        Assert.Equal(sessaoId, dto.Id);
        Assert.Equal("Ep. 1", dto.Titulo);
        Assert.Equal(criadoEm, dto.CriadoEm);
        Assert.Equal(Classificacao.Gostei, dto.Classificacao);
        Assert.Equal(8.5m, dto.Nota);
        Assert.Equal("Ótimo episódio", dto.Anotacoes);
    }
}
