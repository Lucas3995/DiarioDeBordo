using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Core.Enums;
using DiarioDeBordo.Core.Repositorios;
using DiarioDeBordo.Module.Acervo.Commands;
using NSubstitute;

namespace DiarioDeBordo.Tests.Domain.Acervo;

public class AdicionarItemNaColetaneaHandlerTests
{
    private readonly IColetaneaRepository _coletaneaRepo = Substitute.For<IColetaneaRepository>();
    private readonly IConteudoRepository _conteudoRepo = Substitute.For<IConteudoRepository>();
    private readonly AdicionarItemNaColetaneaHandler _handler;

    public AdicionarItemNaColetaneaHandlerTests()
    {
        _handler = new AdicionarItemNaColetaneaHandler(_coletaneaRepo, _conteudoRepo);
    }

    [Fact]
    public async Task Given_ValidItem_When_AdicionarNaColetanea_Then_RetornaSucesso()
    {
        var usuarioId = Guid.NewGuid();
        var coletaneaId = Guid.NewGuid();
        var conteudoId = Guid.NewGuid();

        _coletaneaRepo.ObterPorIdAsync(coletaneaId, usuarioId, Arg.Any<CancellationToken>())
            .Returns(Conteudo.Criar(usuarioId, "Colecao", PapelConteudo.Coletanea, TipoColetanea.Guiada));
        _conteudoRepo.ObterPorIdAsync(conteudoId, usuarioId, Arg.Any<CancellationToken>())
            .Returns(Conteudo.Criar(usuarioId, "Item"));
        _coletaneaRepo.ItemExisteNaColetaneaAsync(coletaneaId, conteudoId, Arg.Any<CancellationToken>())
            .Returns(false);
        _coletaneaRepo.ObterProximaPosicaoAsync(coletaneaId, Arg.Any<CancellationToken>())
            .Returns(3);

        var resultado = await _handler.Handle(
            new AdicionarItemNaColetaneaCommand(coletaneaId, conteudoId, usuarioId),
            CancellationToken.None);

        Assert.True(resultado.IsSuccess);
        await _coletaneaRepo.Received(1).AdicionarItemAsync(
            Arg.Is<ConteudoColetanea>(i =>
                i.ColetaneaId == coletaneaId &&
                i.ConteudoId == conteudoId &&
                i.Posicao == 3),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Given_ItemJaNaColetanea_When_Adicionar_Then_RetornaFalha()
    {
        var usuarioId = Guid.NewGuid();
        var coletaneaId = Guid.NewGuid();
        var conteudoId = Guid.NewGuid();

        _coletaneaRepo.ObterPorIdAsync(coletaneaId, usuarioId, Arg.Any<CancellationToken>())
            .Returns(Conteudo.Criar(usuarioId, "Colecao", PapelConteudo.Coletanea, TipoColetanea.Miscelanea));
        _conteudoRepo.ObterPorIdAsync(conteudoId, usuarioId, Arg.Any<CancellationToken>())
            .Returns(Conteudo.Criar(usuarioId, "Item"));
        _coletaneaRepo.ItemExisteNaColetaneaAsync(coletaneaId, conteudoId, Arg.Any<CancellationToken>())
            .Returns(true);

        var resultado = await _handler.Handle(
            new AdicionarItemNaColetaneaCommand(coletaneaId, conteudoId, usuarioId),
            CancellationToken.None);

        Assert.False(resultado.IsSuccess);
        Assert.Equal("ITEM_JA_NA_COLETANEA", resultado.Error!.Codigo);
    }

    [Fact]
    public async Task Given_ColetaneaCriandoCiclo_When_Adicionar_Then_RetornaFalhaCiclo()
    {
        var usuarioId = Guid.NewGuid();
        var coletaneaId = Guid.NewGuid();
        var conteudoId = Guid.NewGuid();

        _coletaneaRepo.ObterPorIdAsync(coletaneaId, usuarioId, Arg.Any<CancellationToken>())
            .Returns(Conteudo.Criar(usuarioId, "Colecao", PapelConteudo.Coletanea, TipoColetanea.Guiada));
        _conteudoRepo.ObterPorIdAsync(conteudoId, usuarioId, Arg.Any<CancellationToken>())
            .Returns(Conteudo.Criar(usuarioId, "Sub", PapelConteudo.Coletanea, TipoColetanea.Miscelanea));
        _coletaneaRepo.ItemExisteNaColetaneaAsync(coletaneaId, conteudoId, Arg.Any<CancellationToken>())
            .Returns(false);
        _coletaneaRepo.ObterDescendentesAsync(conteudoId, usuarioId, Arg.Any<CancellationToken>())
            .Returns([coletaneaId]);

        var resultado = await _handler.Handle(
            new AdicionarItemNaColetaneaCommand(coletaneaId, conteudoId, usuarioId),
            CancellationToken.None);

        Assert.False(resultado.IsSuccess);
        Assert.Equal("CICLO_COMPOSICAO", resultado.Error!.Codigo);
    }

    [Fact]
    public async Task Given_ColetaneaInexistente_When_Adicionar_Then_RetornaFalha()
    {
        var usuarioId = Guid.NewGuid();
        var coletaneaId = Guid.NewGuid();
        var conteudoId = Guid.NewGuid();

        _coletaneaRepo.ObterPorIdAsync(coletaneaId, usuarioId, Arg.Any<CancellationToken>())
            .Returns((Conteudo?)null);

        var resultado = await _handler.Handle(
            new AdicionarItemNaColetaneaCommand(coletaneaId, conteudoId, usuarioId),
            CancellationToken.None);

        Assert.False(resultado.IsSuccess);
        Assert.Equal("COLETANEA_NAO_ENCONTRADA", resultado.Error!.Codigo);
    }
}
