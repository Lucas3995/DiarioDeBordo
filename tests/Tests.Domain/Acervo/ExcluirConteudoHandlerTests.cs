using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Core.Repositorios;
using DiarioDeBordo.Module.Acervo.Commands;
using MediatR;
using NSubstitute;

namespace DiarioDeBordo.Tests.Domain.Acervo;

/// <summary>
/// Testes do handler ExcluirConteudoHandler em isolamento (NSubstitute).
/// </summary>
public class ExcluirConteudoHandlerTests
{
    private readonly IConteudoRepository _conteudoRepo = Substitute.For<IConteudoRepository>();
    private readonly IRelacaoRepository _relacaoRepo = Substitute.For<IRelacaoRepository>();
    private readonly ExcluirConteudoHandler _handler;
    private static readonly Guid _usuarioId = Guid.NewGuid();

    public ExcluirConteudoHandlerTests()
    {
        _handler = new ExcluirConteudoHandler(_conteudoRepo, _relacaoRepo);
    }

    [Fact]
    public async Task Given_ConteudoNaoEncontrado_When_Excluir_Then_RetornaFailure()
    {
        var id = Guid.NewGuid();
        _conteudoRepo.ObterPorIdAsync(id, _usuarioId, Arg.Any<CancellationToken>()).Returns((Conteudo?)null);

        var resultado = await _handler.Handle(new ExcluirConteudoCommand(id, _usuarioId), CancellationToken.None);

        Assert.False(resultado.IsSuccess);
        Assert.Equal("NAO_ENCONTRADO", resultado.Error!.Codigo);
    }

    [Fact]
    public async Task Given_ConteudoEncontrado_When_Excluir_Then_RetornaSuccess()
    {
        var id = Guid.NewGuid();
        var conteudo = Conteudo.Criar(_usuarioId, "Dune");
        _conteudoRepo.ObterPorIdAsync(id, _usuarioId, Arg.Any<CancellationToken>()).Returns(conteudo);
        _conteudoRepo.ListarFilhosAsync(id, _usuarioId, Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns([]);
        _relacaoRepo.ListarPorConteudoAsync(id, _usuarioId, Arg.Any<CancellationToken>()).Returns([]);

        var resultado = await _handler.Handle(new ExcluirConteudoCommand(id, _usuarioId), CancellationToken.None);

        Assert.True(resultado.IsSuccess);
        Assert.Equal(Unit.Value, resultado.Value);
    }

    [Fact]
    public async Task Given_ConteudoEncontrado_When_Excluir_Then_ChamaRemoverAsync()
    {
        var id = Guid.NewGuid();
        _conteudoRepo.ObterPorIdAsync(id, _usuarioId, Arg.Any<CancellationToken>()).Returns(Conteudo.Criar(_usuarioId, "Dune"));
        _conteudoRepo.ListarFilhosAsync(id, _usuarioId, Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns([]);
        _relacaoRepo.ListarPorConteudoAsync(id, _usuarioId, Arg.Any<CancellationToken>()).Returns([]);

        await _handler.Handle(new ExcluirConteudoCommand(id, _usuarioId), CancellationToken.None);

        await _conteudoRepo.Received(1).RemoverAsync(id, _usuarioId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Given_ConteudoComRelacoes_When_Excluir_Then_RemoveRelacoes()
    {
        var id = Guid.NewGuid();
        var relacaoId = Guid.NewGuid();
        var relacao = new Relacao
        {
            Id = relacaoId, ConteudoOrigemId = id, ConteudoDestinoId = Guid.NewGuid(),
            TipoRelacaoId = Guid.NewGuid(), IsInversa = false, UsuarioId = _usuarioId, CriadoEm = DateTimeOffset.UtcNow,
        };

        _conteudoRepo.ObterPorIdAsync(id, _usuarioId, Arg.Any<CancellationToken>()).Returns(Conteudo.Criar(_usuarioId, "Dune"));
        _conteudoRepo.ListarFilhosAsync(id, _usuarioId, Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns([]);
        _relacaoRepo.ListarPorConteudoAsync(id, _usuarioId, Arg.Any<CancellationToken>()).Returns([relacao]);

        await _handler.Handle(new ExcluirConteudoCommand(id, _usuarioId), CancellationToken.None);

        await _relacaoRepo.Received(1).RemoverParAsync(relacaoId, _usuarioId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Given_ConteudoComFilhos_When_Excluir_Then_RemoveFilhosFirst()
    {
        var id = Guid.NewGuid();
        var filhoId = Guid.NewGuid();

        _conteudoRepo.ObterPorIdAsync(id, _usuarioId, Arg.Any<CancellationToken>()).Returns(Conteudo.Criar(_usuarioId, "Dune"));
        _conteudoRepo.ListarFilhosAsync(id, _usuarioId, Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns([filhoId]);
        _relacaoRepo.ListarPorConteudoAsync(Arg.Any<Guid>(), _usuarioId, Arg.Any<CancellationToken>()).Returns([]);

        await _handler.Handle(new ExcluirConteudoCommand(id, _usuarioId), CancellationToken.None);

        // Should remove the child AND the parent
        await _conteudoRepo.Received(1).RemoverAsync(filhoId, _usuarioId, Arg.Any<CancellationToken>());
        await _conteudoRepo.Received(1).RemoverAsync(id, _usuarioId, Arg.Any<CancellationToken>());
    }
}
