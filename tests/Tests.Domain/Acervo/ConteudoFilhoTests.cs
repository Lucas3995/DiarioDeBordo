using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Core.Enums;
using DiarioDeBordo.Core.Primitivos;

namespace DiarioDeBordo.Tests.Domain.Acervo;

/// <summary>
/// Testes da fábrica Conteudo.CriarComoFilho() e DefinirTotalEsperadoSessoes.
/// Verifica D-17/D-19/D-21.
/// </summary>
public class ConteudoFilhoTests
{
    private static readonly Guid _usuarioId = Guid.NewGuid();

    // ---- CriarComoFilho ----

    [Fact]
    public void Given_TituloValido_When_CriarComoFilho_Then_IsFilhoEhTrue()
    {
        var filho = Conteudo.CriarComoFilho(_usuarioId, "Episódio 1");

        Assert.True(filho.IsFilho);
    }

    [Fact]
    public void Given_TituloValido_When_CriarComoFilho_Then_PapelEhItem()
    {
        var filho = Conteudo.CriarComoFilho(_usuarioId, "Episódio 1");

        Assert.Equal(PapelConteudo.Item, filho.Papel);
    }

    [Fact]
    public void Given_TituloValido_When_CriarComoFilho_Then_IdNaoVazio()
    {
        var filho = Conteudo.CriarComoFilho(_usuarioId, "Episódio 1");

        Assert.NotEqual(Guid.Empty, filho.Id);
    }

    [Fact]
    public void Given_TituloValido_When_CriarComoFilho_Then_UsuarioIdCorreto()
    {
        var filho = Conteudo.CriarComoFilho(_usuarioId, "Episódio 1");

        Assert.Equal(_usuarioId, filho.UsuarioId);
    }

    [Fact]
    public void Given_TituloComEspacos_When_CriarComoFilho_Then_TituloTrimado()
    {
        var filho = Conteudo.CriarComoFilho(_usuarioId, "  Episódio 1  ");

        Assert.Equal("Episódio 1", filho.Titulo);
    }

    [Fact]
    public void Given_TituloVazio_When_CriarComoFilho_Then_LancaDomainException()
    {
        var ex = Assert.Throws<DomainException>(() =>
            Conteudo.CriarComoFilho(_usuarioId, ""));

        Assert.Equal("TITULO_OBRIGATORIO", ex.Codigo);
    }

    [Fact]
    public void Given_TituloSomenteEspacos_When_CriarComoFilho_Then_LancaDomainException()
    {
        var ex = Assert.Throws<DomainException>(() =>
            Conteudo.CriarComoFilho(_usuarioId, "   "));

        Assert.Equal("TITULO_OBRIGATORIO", ex.Codigo);
    }

    [Fact]
    public void Given_FormatoDefined_When_CriarComoFilho_Then_FormatoSetado()
    {
        var filho = Conteudo.CriarComoFilho(_usuarioId, "Episódio 1", FormatoMidia.Video);

        Assert.Equal(FormatoMidia.Video, filho.Formato);
    }

    [Fact]
    public void Given_ConteudoCriado_When_Criar_Then_IsFilhoEhFalse()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "Breaking Bad");

        Assert.False(conteudo.IsFilho);
    }

    [Fact]
    public void Given_DoisFilhosCriados_When_MesmoTitulo_Then_IdsDistintos()
    {
        var filho1 = Conteudo.CriarComoFilho(_usuarioId, "Episódio 1");
        var filho2 = Conteudo.CriarComoFilho(_usuarioId, "Episódio 1");

        Assert.NotEqual(filho1.Id, filho2.Id);
    }

    // ---- DefinirTotalEsperadoSessoes ----

    [Fact]
    public void Given_ValorPositivo_When_DefinirTotalEsperadoSessoes_Then_ValorSetado()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "Breaking Bad");

        conteudo.DefinirTotalEsperadoSessoes(62);

        Assert.Equal(62, conteudo.TotalEsperadoSessoes);
    }

    [Fact]
    public void Given_ValorUm_When_DefinirTotalEsperadoSessoes_Then_ValorSetado()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "Filme");

        conteudo.DefinirTotalEsperadoSessoes(1);

        Assert.Equal(1, conteudo.TotalEsperadoSessoes);
    }

    [Fact]
    public void Given_ValorNull_When_DefinirTotalEsperadoSessoes_Then_CampoLimpo()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "Breaking Bad");
        conteudo.DefinirTotalEsperadoSessoes(62);

        conteudo.DefinirTotalEsperadoSessoes(null);

        Assert.Null(conteudo.TotalEsperadoSessoes);
    }

    [Fact]
    public void Given_ValorZero_When_DefinirTotalEsperadoSessoes_Then_LancaDomainException()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "Breaking Bad");

        var ex = Assert.Throws<DomainException>(() =>
            conteudo.DefinirTotalEsperadoSessoes(0));

        Assert.Equal("TOTAL_ESPERADO_INVALIDO", ex.Codigo);
    }

    [Fact]
    public void Given_ValorNegativo_When_DefinirTotalEsperadoSessoes_Then_LancaDomainException()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "Breaking Bad");

        var ex = Assert.Throws<DomainException>(() =>
            conteudo.DefinirTotalEsperadoSessoes(-5));

        Assert.Equal("TOTAL_ESPERADO_INVALIDO", ex.Codigo);
    }

    [Fact]
    public void Given_TotalSetado_When_DefinirTotalEsperadoSessoes_Then_AtualizadoEmAlterado()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "Breaking Bad");
        System.Threading.Thread.Sleep(5);
        var antes = conteudo.AtualizadoEm;

        conteudo.DefinirTotalEsperadoSessoes(62);

        Assert.True(conteudo.AtualizadoEm >= antes);
    }

    [Fact]
    public void Given_ConteudoCriado_When_SemTotal_Then_TotalEsperadoNuloInicialmente()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "Breaking Bad");

        Assert.Null(conteudo.TotalEsperadoSessoes);
    }
}
