using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Core.Repositorios;
using DiarioDeBordo.Module.Acervo.Commands;
using NSubstitute;

namespace DiarioDeBordo.Tests.Domain.Acervo;

/// <summary>
/// Testes do handler CriarRelacaoHandler em isolamento (NSubstitute).
/// </summary>
public class CriarRelacaoHandlerTests
{
    private readonly IConteudoRepository _conteudoRepo = Substitute.For<IConteudoRepository>();
    private readonly IRelacaoRepository _relacaoRepo = Substitute.For<IRelacaoRepository>();
    private readonly ITipoRelacaoRepository _tipoRelacaoRepo = Substitute.For<ITipoRelacaoRepository>();
    private readonly CriarRelacaoHandler _handler;
    private static readonly Guid _usuarioId = Guid.NewGuid();

    public CriarRelacaoHandlerTests()
    {
        _handler = new CriarRelacaoHandler(_conteudoRepo, _relacaoRepo, _tipoRelacaoRepo);
    }

    private static TipoRelacao TipoFake(string nome = "Sequência", string inverso = "Continuação de") =>
        TipoRelacao.Criar(_usuarioId, nome, inverso);

    [Fact]
    public async Task Given_SameOrigemAndDestino_When_CriarRelacao_Then_ReturnsAutoReferenciaProibida()
    {
        var id = Guid.NewGuid();
        var cmd = new CriarRelacaoCommand(_usuarioId, id, id, "Sequência", "Continuação de");

        var resultado = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(resultado.IsSuccess);
        Assert.Equal("AUTO_REFERENCIA_PROIBIDA", resultado.Error!.Codigo);
        await _conteudoRepo.DidNotReceive().ObterPorIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Given_NonExistentOrigem_When_CriarRelacao_Then_ReturnsNaoEncontrado()
    {
        var origemId = Guid.NewGuid();
        var destinoId = Guid.NewGuid();
        _conteudoRepo.ObterPorIdAsync(origemId, _usuarioId, Arg.Any<CancellationToken>()).Returns((Conteudo?)null);

        var resultado = await _handler.Handle(
            new CriarRelacaoCommand(_usuarioId, origemId, destinoId, "Sequência", "Continuação de"),
            CancellationToken.None);

        Assert.False(resultado.IsSuccess);
        Assert.Equal("NAO_ENCONTRADO", resultado.Error!.Codigo);
    }

    [Fact]
    public async Task Given_NonExistentDestino_When_CriarRelacao_Then_ReturnsNaoEncontrado()
    {
        var origemId = Guid.NewGuid();
        var destinoId = Guid.NewGuid();
        _conteudoRepo.ObterPorIdAsync(origemId, _usuarioId, Arg.Any<CancellationToken>()).Returns(Conteudo.Criar(_usuarioId, "A"));
        _conteudoRepo.ObterPorIdAsync(destinoId, _usuarioId, Arg.Any<CancellationToken>()).Returns((Conteudo?)null);

        var resultado = await _handler.Handle(
            new CriarRelacaoCommand(_usuarioId, origemId, destinoId, "Sequência", "Continuação de"),
            CancellationToken.None);

        Assert.False(resultado.IsSuccess);
        Assert.Equal("NAO_ENCONTRADO", resultado.Error!.Codigo);
    }

    [Fact]
    public async Task Given_DuplicateRelation_When_CriarRelacao_Then_ReturnsRelacaoDuplicada()
    {
        var origemId = Guid.NewGuid();
        var destinoId = Guid.NewGuid();
        var tipo = TipoFake();
        _conteudoRepo.ObterPorIdAsync(origemId, _usuarioId, Arg.Any<CancellationToken>()).Returns(Conteudo.Criar(_usuarioId, "A"));
        _conteudoRepo.ObterPorIdAsync(destinoId, _usuarioId, Arg.Any<CancellationToken>()).Returns(Conteudo.Criar(_usuarioId, "B"));
        _tipoRelacaoRepo.ObterOuCriarAsync(_usuarioId, Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(tipo);
        _relacaoRepo.ExisteAsync(origemId, destinoId, tipo.Id, _usuarioId, Arg.Any<CancellationToken>()).Returns(true);

        var resultado = await _handler.Handle(
            new CriarRelacaoCommand(_usuarioId, origemId, destinoId, "Sequência", "Continuação de"),
            CancellationToken.None);

        Assert.False(resultado.IsSuccess);
        Assert.Equal("RELACAO_DUPLICADA", resultado.Error!.Codigo);
    }

    [Fact]
    public async Task Given_ValidRelation_When_CriarRelacao_Then_CreatesForwardAndInverse()
    {
        var origemId = Guid.NewGuid();
        var destinoId = Guid.NewGuid();
        var tipo = TipoFake();
        _conteudoRepo.ObterPorIdAsync(origemId, _usuarioId, Arg.Any<CancellationToken>()).Returns(Conteudo.Criar(_usuarioId, "A"));
        _conteudoRepo.ObterPorIdAsync(destinoId, _usuarioId, Arg.Any<CancellationToken>()).Returns(Conteudo.Criar(_usuarioId, "B"));
        _tipoRelacaoRepo.ObterOuCriarAsync(_usuarioId, Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(tipo);
        _relacaoRepo.ExisteAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

        var resultado = await _handler.Handle(
            new CriarRelacaoCommand(_usuarioId, origemId, destinoId, "Sequência", "Continuação de"),
            CancellationToken.None);

        Assert.True(resultado.IsSuccess);
        await _relacaoRepo.Received(1).AdicionarParAsync(
            Arg.Is<Relacao>(r => !r.IsInversa && r.ConteudoOrigemId == origemId),
            Arg.Is<Relacao>(r => r.IsInversa && r.ConteudoOrigemId == destinoId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Given_SymmetricType_When_CriarRelacao_Then_CreatesTwoRows()
    {
        var origemId = Guid.NewGuid();
        var destinoId = Guid.NewGuid();
        var tipo = TipoFake("Alternativa a", "Alternativa a"); // symmetric
        _conteudoRepo.ObterPorIdAsync(origemId, _usuarioId, Arg.Any<CancellationToken>()).Returns(Conteudo.Criar(_usuarioId, "A"));
        _conteudoRepo.ObterPorIdAsync(destinoId, _usuarioId, Arg.Any<CancellationToken>()).Returns(Conteudo.Criar(_usuarioId, "B"));
        _tipoRelacaoRepo.ObterOuCriarAsync(_usuarioId, Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(tipo);
        _relacaoRepo.ExisteAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

        var resultado = await _handler.Handle(
            new CriarRelacaoCommand(_usuarioId, origemId, destinoId, "Alternativa a", "Alternativa a"),
            CancellationToken.None);

        Assert.True(resultado.IsSuccess);
        // Even symmetric types create 2 rows (D-15: always two-row bidirectional)
        await _relacaoRepo.Received(1).AdicionarParAsync(Arg.Any<Relacao>(), Arg.Any<Relacao>(), Arg.Any<CancellationToken>());
    }
}
