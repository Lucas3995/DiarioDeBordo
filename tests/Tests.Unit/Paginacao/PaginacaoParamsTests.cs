using DiarioDeBordo.Core.Primitivos;
using Xunit;

namespace DiarioDeBordo.Tests.Unit.Paginacao;

public class PaginacaoParamsTests
{
    [Fact]
    public void Padrao_PaginaUm_VintePorPagina()
    {
        Assert.Equal(1, PaginacaoParams.Padrao.Pagina);
        Assert.Equal(20, PaginacaoParams.Padrao.ItensPorPagina);
        Assert.Equal(0, PaginacaoParams.Padrao.Offset);
    }

    [Theory]
    [InlineData(0, 20)]
    [InlineData(-1, 20)]
    public void CriarComPaginaMenorQueUm_LancaException(int pagina, int itens)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new PaginacaoParams(pagina, itens));
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(1, 201)]
    [InlineData(1, -5)]
    public void CriarComItensFora_LancaException(int pagina, int itens)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new PaginacaoParams(pagina, itens));
    }

    [Fact]
    public void Offset_CalculadoCorretamente()
    {
        var p = new PaginacaoParams(3, 10);
        Assert.Equal(20, p.Offset); // (3-1) * 10 = 20
    }
}
