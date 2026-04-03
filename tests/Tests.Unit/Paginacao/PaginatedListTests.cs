using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Module.Shared.Paginacao;
using Xunit;

namespace DiarioDeBordo.Tests.Unit.Paginacao;

public class PaginatedListTests
{
    [Fact]
    public void TotalPaginas_CelingDivision()
    {
        var lista = new PaginatedList<int>([1], 21, 1, 20);
        Assert.Equal(2, lista.TotalPaginas); // ceil(21/20) = 2
    }

    [Fact]
    public void TemPaginaAnterior_QuandoPaginaMaiorQueUm()
    {
        var lista = new PaginatedList<int>([1], 100, 2, 20);
        Assert.True(lista.TemPaginaAnterior);
    }

    [Fact]
    public void NaoTemPaginaAnterior_QuandoPrimeiraPagina()
    {
        var lista = new PaginatedList<int>([1], 100, 1, 20);
        Assert.False(lista.TemPaginaAnterior);
    }

    [Fact]
    public void TemProximaPagina_QuandoNaoEhUltima()
    {
        var lista = new PaginatedList<int>([1], 100, 1, 20); // TotalPaginas = 5
        Assert.True(lista.TemProximaPagina);
    }

    [Fact]
    public void NaoTemProximaPagina_QuandoUltimaPagina()
    {
        var lista = new PaginatedList<int>([1], 100, 5, 20); // TotalPaginas = 5
        Assert.False(lista.TemProximaPagina);
    }

    [Fact]
    public void Vazia_ZeroItens_SemPaginas()
    {
        var lista = PaginatedList<int>.Vazia();
        Assert.Equal(0, lista.TotalItems);
        Assert.Equal(0, lista.TotalPaginas);
        Assert.False(lista.TemPaginaAnterior);
        Assert.False(lista.TemProximaPagina);
    }

    [Fact]
    public void FromList_PaginaCorretos()
    {
        var source = Enumerable.Range(1, 25).ToList().AsReadOnly();
        var p = new PaginacaoParams(2, 10);
        var lista = PaginatedList<int>.FromList(source, p);

        Assert.Equal(25, lista.TotalItems);
        Assert.Equal(3, lista.TotalPaginas); // ceil(25/10)
        Assert.Equal(10, lista.Items.Count);
        Assert.Equal(11, lista.Items[0]); // Second page starts at item 11
    }
}
