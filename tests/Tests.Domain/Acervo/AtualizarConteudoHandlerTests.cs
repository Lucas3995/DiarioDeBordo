using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Core.Enums;
using DiarioDeBordo.Core.Repositorios;
using DiarioDeBordo.Module.Acervo.Commands;
using MediatR;
using NSubstitute;

namespace DiarioDeBordo.Tests.Domain.Acervo;

/// <summary>
/// Testes do handler AtualizarConteudoHandler em isolamento (NSubstitute).
/// </summary>
public class AtualizarConteudoHandlerTests
{
    private readonly IConteudoRepository _repo = Substitute.For<IConteudoRepository>();
    private readonly AtualizarConteudoHandler _handler;
    private static readonly Guid _usuarioId = Guid.NewGuid();

    public AtualizarConteudoHandlerTests()
    {
        _handler = new AtualizarConteudoHandler(_repo);
    }

    private static Conteudo CriarConteudoFake() => Conteudo.Criar(_usuarioId, "Titulo Original");

    private static AtualizarConteudoCommand CmdValido(Guid id) => new(
        id, _usuarioId, "Novo Titulo", "Descricao", "Anotacoes", 8m,
        Classificacao.Gostei, FormatoMidia.Texto, "Subtipo",
        EstadoProgresso.EmAndamento, "Cap. 5", 10, []);

    [Fact]
    public async Task Given_TituloVazio_When_Atualizar_Then_RetornaFailure()
    {
        var cmd = new AtualizarConteudoCommand(Guid.NewGuid(), _usuarioId, "", null, null, null, null,
            FormatoMidia.Nenhum, null, EstadoProgresso.NaoIniciado, null, null, []);

        var resultado = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(resultado.IsSuccess);
        Assert.Equal("TITULO_OBRIGATORIO", resultado.Error!.Codigo);
        await _repo.DidNotReceive().AtualizarAsync(Arg.Any<Conteudo>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Given_ConteudoNaoEncontrado_When_Atualizar_Then_RetornaFailure()
    {
        var id = Guid.NewGuid();
        _repo.ObterPorIdAsync(id, _usuarioId, Arg.Any<CancellationToken>()).Returns((Conteudo?)null);

        var resultado = await _handler.Handle(CmdValido(id), CancellationToken.None);

        Assert.False(resultado.IsSuccess);
        Assert.Equal("NAO_ENCONTRADO", resultado.Error!.Codigo);
    }

    [Fact]
    public async Task Given_ConteudoEncontrado_When_Atualizar_Then_RetornaSuccess()
    {
        var id = Guid.NewGuid();
        _repo.ObterPorIdAsync(id, _usuarioId, Arg.Any<CancellationToken>()).Returns(CriarConteudoFake());

        var resultado = await _handler.Handle(CmdValido(id), CancellationToken.None);

        Assert.True(resultado.IsSuccess);
        Assert.Equal(Unit.Value, resultado.Value);
    }

    [Fact]
    public async Task Given_ConteudoEncontrado_When_Atualizar_Then_ChAmaAtualizarAsync()
    {
        var id = Guid.NewGuid();
        _repo.ObterPorIdAsync(id, _usuarioId, Arg.Any<CancellationToken>()).Returns(CriarConteudoFake());

        await _handler.Handle(CmdValido(id), CancellationToken.None);

        await _repo.Received(1).AtualizarAsync(Arg.Any<Conteudo>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Given_NotaForaDaFaixa_When_Atualizar_Then_RetornaFailure()
    {
        var id = Guid.NewGuid();
        _repo.ObterPorIdAsync(id, _usuarioId, Arg.Any<CancellationToken>()).Returns(CriarConteudoFake());
        var cmd = new AtualizarConteudoCommand(id, _usuarioId, "Titulo", null, null, 15m, null,
            FormatoMidia.Nenhum, null, EstadoProgresso.NaoIniciado, null, null, []);

        var resultado = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(resultado.IsSuccess);
        Assert.Equal("NOTA_FORA_DA_FAIXA", resultado.Error!.Codigo);
    }

    [Fact]
    public async Task Given_NotaNula_When_Atualizar_Then_LimpaNotaNoConteudo()
    {
        var conteudo = CriarConteudoFake();
        conteudo.DefinirNota(9m);
        var id = Guid.NewGuid();
        _repo.ObterPorIdAsync(id, _usuarioId, Arg.Any<CancellationToken>()).Returns(conteudo);
        var cmd = new AtualizarConteudoCommand(id, _usuarioId, "Titulo", null, null, null, null,
            FormatoMidia.Nenhum, null, EstadoProgresso.NaoIniciado, null, null, []);

        await _handler.Handle(cmd, CancellationToken.None);

        Assert.Null(conteudo.Nota);
    }

    [Fact]
    public async Task Given_TotalEsperadoSessoesZero_When_Atualizar_Then_RetornaFailure()
    {
        var id = Guid.NewGuid();
        _repo.ObterPorIdAsync(id, _usuarioId, Arg.Any<CancellationToken>()).Returns(CriarConteudoFake());
        var cmd = new AtualizarConteudoCommand(id, _usuarioId, "Titulo", null, null, null, null,
            FormatoMidia.Nenhum, null, EstadoProgresso.NaoIniciado, null, 0, []);

        var resultado = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(resultado.IsSuccess);
        Assert.Equal("TOTAL_ESPERADO_INVALIDO", resultado.Error!.Codigo);
    }

    [Fact]
    public async Task Given_ConteudoEncontrado_When_Atualizar_Then_AtualizarCategorias()
    {
        var id = Guid.NewGuid();
        var categoriaIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        _repo.ObterPorIdAsync(id, _usuarioId, Arg.Any<CancellationToken>()).Returns(CriarConteudoFake());

        var cmd = new AtualizarConteudoCommand(id, _usuarioId, "Titulo", null, null, null, null,
            FormatoMidia.Nenhum, null, EstadoProgresso.NaoIniciado, null, null, categoriaIds.AsReadOnly());

        await _handler.Handle(cmd, CancellationToken.None);

        await _repo.Received(1).AtualizarCategoriasAsync(
            id, _usuarioId,
            Arg.Is<IReadOnlyList<Guid>>(list => list.SequenceEqual(categoriaIds)),
            Arg.Any<CancellationToken>());
    }
}
