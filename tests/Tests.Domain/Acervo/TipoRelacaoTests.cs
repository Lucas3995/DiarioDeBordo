using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Core.Primitivos;

namespace DiarioDeBordo.Tests.Domain.Acervo;

/// <summary>
/// Testes da fábrica TipoRelacao.Criar() e suas invariantes.
/// Verifica D-12/D-13 e deduplicação por NomeNormalizado.
/// </summary>
public class TipoRelacaoTests
{
    private static readonly Guid _usuarioId = Guid.NewGuid();

    [Fact]
    public void Given_NomesValidos_When_Criar_Then_PropriedadesSetadas()
    {
        var tipo = TipoRelacao.Criar(_usuarioId, "Sequência", "Continuação de");

        Assert.Equal(_usuarioId, tipo.UsuarioId);
        Assert.Equal("Sequência", tipo.Nome);
        Assert.Equal("Continuação de", tipo.NomeInverso);
        Assert.NotEqual(Guid.Empty, tipo.Id);
    }

    [Fact]
    public void Given_NomesValidos_When_Criar_Then_NomeNormalizadoEhLowercase()
    {
        var tipo = TipoRelacao.Criar(_usuarioId, "Sequência", "Continuação de");

        Assert.Equal("sequência", tipo.NomeNormalizado);
    }

    [Fact]
    public void Given_NomeComEspacos_When_Criar_Then_NomeTrimado()
    {
        var tipo = TipoRelacao.Criar(_usuarioId, "  Sequência  ", "  Continuação de  ");

        Assert.Equal("Sequência", tipo.Nome);
        Assert.Equal("Continuação de", tipo.NomeInverso);
        Assert.Equal("sequência", tipo.NomeNormalizado);
    }

    [Fact]
    public void Given_IsSistemaTrue_When_Criar_Then_FlagPreservado()
    {
        var tipo = TipoRelacao.Criar(Guid.Empty, "Contém", "Parte de", isSistema: true);

        Assert.True(tipo.IsSistema);
        Assert.Equal(Guid.Empty, tipo.UsuarioId);
    }

    [Fact]
    public void Given_IsSistemaDefault_When_Criar_Then_EhFalse()
    {
        var tipo = TipoRelacao.Criar(_usuarioId, "Derivado de", "Derivou");

        Assert.False(tipo.IsSistema);
    }

    [Fact]
    public void Given_NomeVazio_When_Criar_Then_LancaDomainException()
    {
        var ex = Assert.Throws<DomainException>(() =>
            TipoRelacao.Criar(_usuarioId, "", "Continuação de"));

        Assert.Equal("NOME_TIPO_RELACAO_OBRIGATORIO", ex.Codigo);
    }

    [Fact]
    public void Given_NomeSomenteEspacos_When_Criar_Then_LancaDomainException()
    {
        var ex = Assert.Throws<DomainException>(() =>
            TipoRelacao.Criar(_usuarioId, "   ", "Continuação de"));

        Assert.Equal("NOME_TIPO_RELACAO_OBRIGATORIO", ex.Codigo);
    }

    [Fact]
    public void Given_NomeInversoVazio_When_Criar_Then_LancaDomainException()
    {
        var ex = Assert.Throws<DomainException>(() =>
            TipoRelacao.Criar(_usuarioId, "Sequência", ""));

        Assert.Equal("NOME_INVERSO_OBRIGATORIO", ex.Codigo);
    }

    [Fact]
    public void Given_NomeInversoSomenteEspacos_When_Criar_Then_LancaDomainException()
    {
        var ex = Assert.Throws<DomainException>(() =>
            TipoRelacao.Criar(_usuarioId, "Sequência", "   "));

        Assert.Equal("NOME_INVERSO_OBRIGATORIO", ex.Codigo);
    }

    [Fact]
    public void Given_TipoSimetrico_When_Criar_Then_NomesIguaisPermitido()
    {
        // D-13: "Alternativa a" ↔ "Alternativa a" (simétrico)
        var tipo = TipoRelacao.Criar(_usuarioId, "Alternativa a", "Alternativa a");

        Assert.Equal("Alternativa a", tipo.Nome);
        Assert.Equal("Alternativa a", tipo.NomeInverso);
    }

    [Fact]
    public void Given_DoisCriar_When_MesmoNome_Then_IdsDistintos()
    {
        // Deduplication is responsibility of ITipoRelacaoRepository — factory creates distinct instances
        var tipo1 = TipoRelacao.Criar(_usuarioId, "Sequência", "Continuação de");
        var tipo2 = TipoRelacao.Criar(_usuarioId, "Sequência", "Continuação de");

        Assert.NotEqual(tipo1.Id, tipo2.Id);
        Assert.Equal(tipo1.NomeNormalizado, tipo2.NomeNormalizado);
    }
}
