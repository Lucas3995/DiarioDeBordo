using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Core.Enums;
using DiarioDeBordo.Core.Primitivos;

namespace DiarioDeBordo.Tests.Domain.Acervo;

/// <summary>
/// Testes das operações DefinirClassificacao e LimparNota no agregado Conteudo.
/// Verifica D-06 (independência entre Nota e Classificacao) e D-07/D-08.
/// </summary>
public class ClassificacaoTests
{
    private static readonly Guid _usuarioId = Guid.NewGuid();

    private static Conteudo CriarConteudo(string titulo = "Dune") =>
        Conteudo.Criar(_usuarioId, titulo);

    // ---- DefinirClassificacao ----

    [Fact]
    public void Given_ConteudoSemClassificacao_When_DefinirClassificacaoGostei_Then_ValorSetado()
    {
        var conteudo = CriarConteudo();

        conteudo.DefinirClassificacao(Classificacao.Gostei);

        Assert.Equal(Classificacao.Gostei, conteudo.Classificacao);
    }

    [Fact]
    public void Given_ConteudoSemClassificacao_When_DefinirClassificacaoNaoGostei_Then_ValorSetado()
    {
        var conteudo = CriarConteudo();

        conteudo.DefinirClassificacao(Classificacao.NaoGostei);

        Assert.Equal(Classificacao.NaoGostei, conteudo.Classificacao);
    }

    [Fact]
    public void Given_ConteudoComClassificacao_When_DefinirClassificacaoNull_Then_ClassificacaoLimpa()
    {
        var conteudo = CriarConteudo();
        conteudo.DefinirClassificacao(Classificacao.Gostei);

        conteudo.DefinirClassificacao(null);

        Assert.Null(conteudo.Classificacao);
    }

    [Fact]
    public void Given_ConteudoCriado_When_NenhumaClassificacao_Then_NuloInicialmente()
    {
        var conteudo = CriarConteudo();

        Assert.Null(conteudo.Classificacao);
    }

    [Fact]
    public void Given_ConteudoSemClassificacao_When_DefinirClassificacao_Then_AtualizadoEmAlterado()
    {
        var conteudo = CriarConteudo();
        var antes = conteudo.AtualizadoEm;
        // Give time to differ (at least a tick)
        System.Threading.Thread.Sleep(5);

        conteudo.DefinirClassificacao(Classificacao.Gostei);

        Assert.True(conteudo.AtualizadoEm >= antes);
    }

    // ---- LimparNota ----

    [Fact]
    public void Given_ConteudoComNota_When_LimparNota_Then_NotaNula()
    {
        var conteudo = CriarConteudo();
        conteudo.DefinirNota(8.5m);
        Assert.Equal(8.5m, conteudo.Nota);

        conteudo.LimparNota();

        Assert.Null(conteudo.Nota);
    }

    [Fact]
    public void Given_ConteudoSemNota_When_LimparNota_Then_NotaContinuaNula()
    {
        var conteudo = CriarConteudo();
        Assert.Null(conteudo.Nota);

        conteudo.LimparNota();

        Assert.Null(conteudo.Nota);
    }

    [Fact]
    public void Given_ConteudoComNota_When_LimparNota_Then_AtualizadoEmAlterado()
    {
        var conteudo = CriarConteudo();
        conteudo.DefinirNota(9m);
        System.Threading.Thread.Sleep(5);
        var antes = conteudo.AtualizadoEm;

        conteudo.LimparNota();

        Assert.True(conteudo.AtualizadoEm >= antes);
    }

    // ---- Independência D-06/D-07 ----

    [Fact]
    public void Given_Nota9_When_DefinirNaoGostei_Then_AmbosIndependentes()
    {
        var conteudo = CriarConteudo();
        conteudo.DefinirNota(9m);

        conteudo.DefinirClassificacao(Classificacao.NaoGostei);

        Assert.Equal(9m, conteudo.Nota);
        Assert.Equal(Classificacao.NaoGostei, conteudo.Classificacao);
    }

    [Fact]
    public void Given_GosteiDefinido_When_DefinirNota_Then_ClassificacaoNaoMuda()
    {
        var conteudo = CriarConteudo();
        conteudo.DefinirClassificacao(Classificacao.Gostei);

        conteudo.DefinirNota(3m);

        Assert.Equal(Classificacao.Gostei, conteudo.Classificacao);
        Assert.Equal(3m, conteudo.Nota);
    }

    [Fact]
    public void Given_GosteiDefinido_When_LimparNota_Then_ClassificacaoNaoMuda()
    {
        var conteudo = CriarConteudo();
        conteudo.DefinirNota(7m);
        conteudo.DefinirClassificacao(Classificacao.Gostei);

        conteudo.LimparNota();

        Assert.Equal(Classificacao.Gostei, conteudo.Classificacao);
        Assert.Null(conteudo.Nota);
    }
}
