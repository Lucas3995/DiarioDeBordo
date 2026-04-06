using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Core.Primitivos;

namespace DiarioDeBordo.Tests.Domain.Acervo;

/// <summary>
/// Testes das invariantes I-11 e I-12 do agregado Categoria.
/// I-12 (unicidade case-insensitive) é testada na camada de repositório em Tests.Integration;
/// aqui testamos a criação do value object e a normalização.
/// </summary>
public class CategoriaInvariantTests
{
    private static readonly Guid _usuarioId = Guid.NewGuid();

    // ---- I-11: Nome de categoria obrigatório ----

    [Fact]
    public void CriarCategoria_NomeVazio_LancaException()
    {
        var ex = Assert.Throws<DomainException>(() =>
            Categoria.Criar(_usuarioId, ""));

        Assert.Equal("NOME_CATEGORIA_OBRIGATORIO", ex.Codigo);
    }

    [Fact]
    public void CriarCategoria_NomeSomenteEspacos_LancaException()
    {
        var ex = Assert.Throws<DomainException>(() =>
            Categoria.Criar(_usuarioId, "   "));

        Assert.Equal("NOME_CATEGORIA_OBRIGATORIO", ex.Codigo);
    }

    [Fact]
    public void CriarCategoria_NomeValido_Sucede()
    {
        var categoria = Categoria.Criar(_usuarioId, "Romance");

        Assert.Equal("Romance", categoria.Nome);
        Assert.Equal(_usuarioId, categoria.UsuarioId);
        Assert.NotEqual(Guid.Empty, categoria.Id);
    }

    // ---- I-12: Normalização case-insensitive (verificação do value object) ----

    [Fact]
    public void CriarCategoria_NomeNormalizadoEhSempreMinusculo()
    {
        var categoria = Categoria.Criar(_usuarioId, "Romance");

        Assert.Equal("romance", categoria.NomeNormalizado);
    }

    [Fact]
    public void CriarCategoria_NomeMisto_Normalizado()
    {
        var categoria = Categoria.Criar(_usuarioId, "Ficção Científica");

        Assert.Equal("ficção científica", categoria.NomeNormalizado);
    }

    [Fact]
    public void CriarCategoria_NomeComEspacosNasExtremidades_EhTrimado()
    {
        var categoria = Categoria.Criar(_usuarioId, "  Romance  ");

        Assert.Equal("Romance", categoria.Nome);
        Assert.Equal("romance", categoria.NomeNormalizado);
    }
}
