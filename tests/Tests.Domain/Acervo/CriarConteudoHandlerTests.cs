using DiarioDeBordo.Core.Repositorios;
using DiarioDeBordo.Module.Acervo.Commands;
using MediatR;
using NSubstitute;

namespace DiarioDeBordo.Tests.Domain.Acervo;

/// <summary>
/// Testes do handler CriarConteudoHandler em isolamento.
/// Usa NSubstitute para mock do repositório — sem banco de dados.
/// </summary>
public class CriarConteudoHandlerTests
{
    private readonly IConteudoRepository _repo = Substitute.For<IConteudoRepository>();
    private readonly IPublisher _publisher = Substitute.For<IPublisher>();
    private readonly CriarConteudoHandler _handler;

    public CriarConteudoHandlerTests()
    {
        _handler = new CriarConteudoHandler(_repo, _publisher);
    }

    // ---- I-01: Título vazio → Resultado.Failure ----

    [Fact]
    public async Task Handle_TituloVazio_RetornaFailure()
    {
        var cmd = new CriarConteudoCommand(Guid.NewGuid(), "");

        var resultado = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(resultado.IsSuccess);
        Assert.Equal("TITULO_OBRIGATORIO", resultado.Error!.Codigo);
        await _repo.DidNotReceive().AdicionarAsync(Arg.Any<Core.Entidades.Conteudo>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_TituloSomenteEspacos_RetornaFailure()
    {
        var cmd = new CriarConteudoCommand(Guid.NewGuid(), "   ");

        var resultado = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(resultado.IsSuccess);
        Assert.Equal("TITULO_OBRIGATORIO", resultado.Error!.Codigo);
    }

    [Fact]
    public async Task Handle_TituloValido_RetornaSuccessComGuid()
    {
        var usuarioId = Guid.NewGuid();
        var cmd = new CriarConteudoCommand(usuarioId, "Dune");

        var resultado = await _handler.Handle(cmd, CancellationToken.None);

        Assert.True(resultado.IsSuccess);
        Assert.NotEqual(Guid.Empty, resultado.Value);
    }

    [Fact]
    public async Task Handle_TituloValido_ChamaAdicionarNoRepositorio()
    {
        var usuarioId = Guid.NewGuid();
        var cmd = new CriarConteudoCommand(usuarioId, "Dune");

        await _handler.Handle(cmd, CancellationToken.None);

        await _repo.Received(1).AdicionarAsync(
            Arg.Is<Core.Entidades.Conteudo>(c => c.Titulo == "Dune" && c.UsuarioId == usuarioId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_TituloValido_PublicaConteudoCriadoNotification()
    {
        var usuarioId = Guid.NewGuid();
        var cmd = new CriarConteudoCommand(usuarioId, "Dune");

        await _handler.Handle(cmd, CancellationToken.None);

        await _publisher.Received(1).Publish(
            Arg.Is<Core.Eventos.ConteudoCriadoNotification>(n =>
                n.Titulo == "Dune" && n.UsuarioId == usuarioId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_TituloComEspacos_SalvaComTituloTrimado()
    {
        var cmd = new CriarConteudoCommand(Guid.NewGuid(), "  Dune  ");

        await _handler.Handle(cmd, CancellationToken.None);

        await _repo.Received(1).AdicionarAsync(
            Arg.Is<Core.Entidades.Conteudo>(c => c.Titulo == "Dune"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ComDescricaoEAnotacoes_SalvaAmbosOsCampos()
    {
        var cmd = new CriarConteudoCommand(Guid.NewGuid(), "Dune",
            Descricao: "Planeta areia.", Anotacoes: "Leitura obrigatória");

        await _handler.Handle(cmd, CancellationToken.None);

        await _repo.Received(1).AdicionarAsync(
            Arg.Is<Core.Entidades.Conteudo>(c =>
                c.Descricao == "Planeta areia." && c.Anotacoes == "Leitura obrigatória"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ComDescricaoVazia_NaoSetaDescricao()
    {
        var cmd = new CriarConteudoCommand(Guid.NewGuid(), "Dune",
            Descricao: "   ", Anotacoes: null);

        await _handler.Handle(cmd, CancellationToken.None);

        await _repo.Received(1).AdicionarAsync(
            Arg.Is<Core.Entidades.Conteudo>(c => c.Descricao == null),
            Arg.Any<CancellationToken>());
    }
}
