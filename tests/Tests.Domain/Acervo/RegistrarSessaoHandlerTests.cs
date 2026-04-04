using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Core.Enums;
using DiarioDeBordo.Core.Repositorios;
using DiarioDeBordo.Module.Acervo.Commands;
using NSubstitute;

namespace DiarioDeBordo.Tests.Domain.Acervo;

/// <summary>
/// Testes do handler RegistrarSessaoHandler em isolamento (NSubstitute).
/// </summary>
public class RegistrarSessaoHandlerTests
{
    private readonly IConteudoRepository _conteudoRepo = Substitute.For<IConteudoRepository>();
    private readonly IRelacaoRepository _relacaoRepo = Substitute.For<IRelacaoRepository>();
    private readonly RegistrarSessaoHandler _handler;
    private static readonly Guid _usuarioId = Guid.NewGuid();

    public RegistrarSessaoHandlerTests()
    {
        _handler = new RegistrarSessaoHandler(_conteudoRepo, _relacaoRepo);
    }

    private static RegistrarSessaoCommand CmdValido(Guid paiId, string titulo = "Episódio 1") =>
        new(_usuarioId, paiId, titulo, null, null, null, FormatoMidia.Nenhum, null);

    [Fact]
    public async Task Given_NonExistentParent_When_RegistrarSessao_Then_ReturnsNaoEncontrado()
    {
        var paiId = Guid.NewGuid();
        _conteudoRepo.ObterPorIdAsync(paiId, _usuarioId, Arg.Any<CancellationToken>()).Returns((Conteudo?)null);

        var resultado = await _handler.Handle(CmdValido(paiId), CancellationToken.None);

        Assert.False(resultado.IsSuccess);
        Assert.Equal("NAO_ENCONTRADO", resultado.Error!.Codigo);
    }

    [Fact]
    public async Task Given_ValidParent_When_RegistrarSessao_Then_CreatesChildWithIsFilhoTrue()
    {
        var paiId = Guid.NewGuid();
        _conteudoRepo.ObterPorIdAsync(paiId, _usuarioId, Arg.Any<CancellationToken>()).Returns(Conteudo.Criar(_usuarioId, "Breaking Bad"));

        Conteudo? filhoCriado = null;
        await _conteudoRepo.AdicionarAsync(Arg.Do<Conteudo>(c => filhoCriado = c), Arg.Any<CancellationToken>());

        var resultado = await _handler.Handle(CmdValido(paiId), CancellationToken.None);

        Assert.True(resultado.IsSuccess);
        Assert.NotNull(filhoCriado);
        Assert.True(filhoCriado!.IsFilho);
    }

    [Fact]
    public async Task Given_ValidParent_When_RegistrarSessao_Then_CreatesBidirectionalRelation()
    {
        var paiId = Guid.NewGuid();
        _conteudoRepo.ObterPorIdAsync(paiId, _usuarioId, Arg.Any<CancellationToken>()).Returns(Conteudo.Criar(_usuarioId, "Breaking Bad"));

        await _handler.Handle(CmdValido(paiId), CancellationToken.None);

        await _relacaoRepo.Received(1).AdicionarParAsync(
            Arg.Is<Relacao>(r => !r.IsInversa && r.ConteudoOrigemId == paiId),
            Arg.Is<Relacao>(r => r.IsInversa && r.ConteudoDestinoId == paiId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Given_NotaEClassificacao_When_RegistrarSessao_Then_SetsOnChild()
    {
        var paiId = Guid.NewGuid();
        _conteudoRepo.ObterPorIdAsync(paiId, _usuarioId, Arg.Any<CancellationToken>()).Returns(Conteudo.Criar(_usuarioId, "Breaking Bad"));

        Conteudo? filhoCriado = null;
        await _conteudoRepo.AdicionarAsync(Arg.Do<Conteudo>(c => filhoCriado = c), Arg.Any<CancellationToken>());

        var cmd = new RegistrarSessaoCommand(_usuarioId, paiId, "Ep 1", "Notas", 9m, Classificacao.Gostei, FormatoMidia.Video, null);
        var resultado = await _handler.Handle(cmd, CancellationToken.None);

        Assert.True(resultado.IsSuccess);
        Assert.Equal(9m, filhoCriado!.Nota);
        Assert.Equal(Classificacao.Gostei, filhoCriado.Classificacao);
    }

    [Fact]
    public async Task Given_TituloVazio_When_RegistrarSessao_Then_ReturnsFailure()
    {
        var paiId = Guid.NewGuid();
        _conteudoRepo.ObterPorIdAsync(paiId, _usuarioId, Arg.Any<CancellationToken>()).Returns(Conteudo.Criar(_usuarioId, "Breaking Bad"));

        var resultado = await _handler.Handle(CmdValido(paiId, ""), CancellationToken.None);

        Assert.False(resultado.IsSuccess);
        Assert.Equal("TITULO_OBRIGATORIO", resultado.Error!.Codigo);
    }

    [Fact]
    public async Task Given_DataConsumo_When_RegistrarSessao_Then_SetsCriadoEm()
    {
        var paiId = Guid.NewGuid();
        var dataConsumo = new DateTimeOffset(2024, 1, 15, 20, 0, 0, TimeSpan.Zero);
        _conteudoRepo.ObterPorIdAsync(paiId, _usuarioId, Arg.Any<CancellationToken>()).Returns(Conteudo.Criar(_usuarioId, "Breaking Bad"));

        Conteudo? filhoCriado = null;
        await _conteudoRepo.AdicionarAsync(Arg.Do<Conteudo>(c => filhoCriado = c), Arg.Any<CancellationToken>());

        var cmd = new RegistrarSessaoCommand(_usuarioId, paiId, "Ep 1", null, null, null, FormatoMidia.Nenhum, dataConsumo);
        await _handler.Handle(cmd, CancellationToken.None);

        Assert.Equal(dataConsumo, filhoCriado!.CriadoEm);
    }
}
