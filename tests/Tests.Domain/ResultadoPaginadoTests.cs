using DiarioDeBordo.Core.Primitivos;

namespace DiarioDeBordo.Tests.Domain;

/// <summary>
/// Testes do tipo ResultadoPaginado&lt;T&gt; — propriedades computadas de navegação.
/// </summary>
public class ResultadoPaginadoTests
{
    [Fact]
    public void TemPaginaAnterior_QuandoPaginaMaiorQueUm_Verdadeiro()
    {
        var resultado = new ResultadoPaginado<string>(
            Array.Empty<string>().AsReadOnly(), totalItems: 50, paginaAtual: 2, tamanhoPagina: 10);

        Assert.True(resultado.TemPaginaAnterior);
    }

    [Fact]
    public void TemPaginaAnterior_QuandoPrimeiraPagina_Falso()
    {
        var resultado = new ResultadoPaginado<string>(
            Array.Empty<string>().AsReadOnly(), totalItems: 50, paginaAtual: 1, tamanhoPagina: 10);

        Assert.False(resultado.TemPaginaAnterior);
    }

    [Fact]
    public void TemProximaPagina_QuandoNaoEhUltimaPagina_Verdadeiro()
    {
        var resultado = new ResultadoPaginado<string>(
            Array.Empty<string>().AsReadOnly(), totalItems: 50, paginaAtual: 1, tamanhoPagina: 10);

        Assert.True(resultado.TemProximaPagina); // 5 páginas, está na 1
    }

    [Fact]
    public void TemProximaPagina_QuandoUltimaPagina_Falso()
    {
        var resultado = new ResultadoPaginado<string>(
            Array.Empty<string>().AsReadOnly(), totalItems: 50, paginaAtual: 5, tamanhoPagina: 10);

        Assert.False(resultado.TemProximaPagina);
    }

    [Fact]
    public void TotalPaginas_CalculaCeiling()
    {
        var resultado = new ResultadoPaginado<int>(
            Array.Empty<int>().AsReadOnly(), totalItems: 21, paginaAtual: 1, tamanhoPagina: 10);

        Assert.Equal(3, resultado.TotalPaginas); // ceil(21/10) = 3
    }

    [Fact]
    public void ResultadoVazio_SemPaginas()
    {
        var resultado = new ResultadoPaginado<int>(
            Array.Empty<int>().AsReadOnly(), totalItems: 0, paginaAtual: 1, tamanhoPagina: 10);

        Assert.Equal(0, resultado.TotalPaginas);
        Assert.False(resultado.TemPaginaAnterior);
        Assert.False(resultado.TemProximaPagina);
    }
}
