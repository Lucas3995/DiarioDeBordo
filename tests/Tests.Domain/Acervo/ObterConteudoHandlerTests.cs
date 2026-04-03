using DiarioDeBordo.Core.Consultas;
using DiarioDeBordo.Core.Enums;
using DiarioDeBordo.Module.Acervo.Queries;
using NSubstitute;

namespace DiarioDeBordo.Tests.Domain.Acervo;

/// <summary>
/// Testes unitários do handler ObterConteudoHandler.
/// Usa NSubstitute para mock do IConteudoQueryService — sem banco de dados.
/// </summary>
public class ObterConteudoHandlerTests
{
    private readonly IConteudoQueryService _queryService = Substitute.For<IConteudoQueryService>();
    private readonly ObterConteudoHandler _handler;

    public ObterConteudoHandlerTests()
    {
        _handler = new ObterConteudoHandler(_queryService);
    }

    private static ConteudoDetalheData MakeDetalhe(Guid id, string titulo, string? descricao = null, string? anotacoes = null, decimal? nota = null, FormatoMidia formato = FormatoMidia.Texto)
        => new(id, titulo, descricao, anotacoes, nota, formato, PapelConteudo.Item, DateTimeOffset.UtcNow,
            null, false, null, null, EstadoProgresso.NaoIniciado, null,
            [], [], [], 0);

    [Fact]
    public async Task Handle_ConteudoNaoEncontrado_RetornaFalha()
    {
        _queryService
            .ObterAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((ConteudoDetalheData?)null);

        var result = await _handler.Handle(
            new ObterConteudoQuery(Guid.NewGuid(), Guid.NewGuid()),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("NAO_ENCONTRADO", result.Error!.Codigo);
    }

    [Fact]
    public async Task Handle_ConteudoEncontrado_RetornaDetalheCompleto()
    {
        var id = Guid.NewGuid();
        var usuarioId = Guid.NewGuid();
        var data = MakeDetalhe(id, "Duna", "Descricao", "Anotação", 9.0m);

        _queryService
            .ObterAsync(id, usuarioId, Arg.Any<CancellationToken>())
            .Returns(data);

        var result = await _handler.Handle(
            new ObterConteudoQuery(id, usuarioId),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value!.Id);
        Assert.Equal("Duna", result.Value.Titulo);
        Assert.Equal("Descricao", result.Value.Descricao);
        Assert.Equal("Anotação", result.Value.Anotacoes);
        Assert.Equal(9.0m, result.Value.Nota);
        Assert.Equal("Texto", result.Value.Formato);
        Assert.Equal("Item", result.Value.Papel);
    }

    [Fact]
    public async Task Handle_ConteudoComCamposOpcionaisNulos_RetornaDetalhe()
    {
        var id = Guid.NewGuid();
        var usuarioId = Guid.NewGuid();
        var data = MakeDetalhe(id, "Sem Descrição", formato: FormatoMidia.Video);

        _queryService
            .ObterAsync(id, usuarioId, Arg.Any<CancellationToken>())
            .Returns(data);

        var result = await _handler.Handle(
            new ObterConteudoQuery(id, usuarioId),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value!.Descricao);
        Assert.Null(result.Value.Anotacoes);
        Assert.Null(result.Value.Nota);
    }

    [Fact]
    public async Task Handle_IsolaUsuario_NaoRetornaConteudoDeOutroUsuario()
    {
        var id = Guid.NewGuid();
        var usuarioCorreto = Guid.NewGuid();
        var outroUsuario = Guid.NewGuid();

        _queryService
            .ObterAsync(id, outroUsuario, Arg.Any<CancellationToken>())
            .Returns((ConteudoDetalheData?)null);

        var result = await _handler.Handle(
            new ObterConteudoQuery(id, outroUsuario),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        await _queryService.Received(1).ObterAsync(id, outroUsuario, Arg.Any<CancellationToken>());
        await _queryService.DidNotReceive().ObterAsync(id, usuarioCorreto, Arg.Any<CancellationToken>());
    }
}
