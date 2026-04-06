using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Core.Repositorios;
using DiarioDeBordo.Module.Acervo.Commands;
using NSubstitute;

namespace DiarioDeBordo.Tests.Domain.Acervo;

/// <summary>
/// Testes de ObterOuCriarCategoriaHandler e RemoverRelacaoHandler.
/// </summary>
public class CategoriaERelacaoHandlersTests
{
    private static readonly Guid _usuarioId = Guid.NewGuid();

    // ── ObterOuCriarCategoriaHandler ─────────────────────────────────────────

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ObterOuCriarCategoria_NomeVazio_RetornaTituloObrigatorio(string nome)
    {
        var repo = Substitute.For<ICategoriaRepository>();
        var handler = new ObterOuCriarCategoriaHandler(repo);

        var result = await handler.Handle(new ObterOuCriarCategoriaCommand(_usuarioId, nome), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("TITULO_OBRIGATORIO", result.Error!.Codigo);
        await repo.DidNotReceive().ObterOuCriarAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ObterOuCriarCategoria_NomeValido_RetornaDtoComIsAutomaticaFalse()
    {
        // Categorias criadas inline pelo usuário nunca são automáticas (D-10)
        var repo = Substitute.For<ICategoriaRepository>();
        var categoria = Categoria.Criar(_usuarioId, "Terror");
        repo.ObterOuCriarAsync(_usuarioId, "Terror", Arg.Any<CancellationToken>()).Returns(categoria);

        var result = await new ObterOuCriarCategoriaHandler(repo)
            .Handle(new ObterOuCriarCategoriaCommand(_usuarioId, "Terror"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Terror", result.Value!.Nome);
        Assert.False(result.Value.IsAutomatica);
    }

    // ── RemoverRelacaoHandler ─────────────────────────────────────────────────

    [Fact]
    public async Task RemoverRelacao_DelegaAoRepositorioComOsIdsCorretos()
    {
        // O handler é fino — a lógica de remover ambos os lados está no repository (SEG-02 garantido lá)
        var relacaoRepo = Substitute.For<IRelacaoRepository>();
        var relacaoId = Guid.NewGuid();

        var result = await new RemoverRelacaoHandler(relacaoRepo)
            .Handle(new RemoverRelacaoCommand(relacaoId, _usuarioId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        await relacaoRepo.Received(1).RemoverParAsync(relacaoId, _usuarioId, Arg.Any<CancellationToken>());
    }
}
