using DiarioDeBordo.Core.Primitivos;

namespace DiarioDeBordo.Tests.Domain;

/// <summary>
/// Testes do tipo Resultado&lt;T&gt; — Railway Oriented Programming.
/// Baseado em: Wlaschin, S. (2014). Railway Oriented Programming.
/// </summary>
public class ResultadoTests
{
    [Fact]
    public void Failure_PropagaErroDeOutroTipo()
    {
        var original = Resultado<string>.Failure(new Erro("ERRO_X", "Mensagem X"));
        var propagado = Resultado<int>.Failure(original);

        Assert.False(propagado.IsSuccess);
        Assert.Equal("ERRO_X", propagado.Error!.Codigo);
        Assert.Equal("Mensagem X", propagado.Error.Mensagem);
    }

    [Fact]
    public void Failure_ResultadoSucesso_LancaInvalidOperationException()
    {
        var sucesso = Resultado<string>.Success("valor");

        Assert.Throws<InvalidOperationException>(() =>
            Resultado<int>.Failure(sucesso));
    }

    [Fact]
    public void Failure_ArgumentoNulo_LancaArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Resultado<int>.Failure<string>(null!));
    }

    [Fact]
    public void Success_IsSuccess_True()
    {
        var result = Resultado<int>.Success(42);

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(42, result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Failure_IsFailure_True()
    {
        var erro = new Erro("CODIGO", "Mensagem");
        var result = Resultado<int>.Failure(erro);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(erro, result.Error);
        Assert.Equal(default, result.Value);
    }
}
