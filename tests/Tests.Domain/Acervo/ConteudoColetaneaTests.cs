using DiarioDeBordo.Core.Entidades;

namespace DiarioDeBordo.Tests.Domain.Acervo;

/// <summary>
/// Testes da entidade ConteudoColetanea — join entity entre coletânea e conteúdo.
/// ACE-08: AnotacaoContextual pertence à relação, não ao conteúdo.
/// </summary>
public class ConteudoColetaneaTests
{
    [Fact]
    public void Given_ValidData_When_ConteudoColetaneaCriado_Then_PropertiesSetCorrectly()
    {
        var coletaneaId = Guid.NewGuid();
        var conteudoId = Guid.NewGuid();
        var agora = DateTimeOffset.UtcNow;

        var item = new ConteudoColetanea
        {
            ColetaneaId = coletaneaId,
            ConteudoId = conteudoId,
            Posicao = 1,
            AnotacaoContextual = null,
            AdicionadoEm = agora
        };

        Assert.Equal(coletaneaId, item.ColetaneaId);
        Assert.Equal(conteudoId, item.ConteudoId);
        Assert.Equal(1, item.Posicao);
        Assert.Null(item.AnotacaoContextual);
        Assert.Equal(agora, item.AdicionadoEm);
    }

    [Fact]
    public void Given_ConteudoColetanea_When_AnotacaoContextualDefinida_Then_ValorPersistido()
    {
        var item = new ConteudoColetanea
        {
            ColetaneaId = Guid.NewGuid(),
            ConteudoId = Guid.NewGuid(),
            Posicao = 1,
            AnotacaoContextual = null,
            AdicionadoEm = DateTimeOffset.UtcNow
        };

        item.AnotacaoContextual = "Nota sobre este item na coletânea";

        Assert.Equal("Nota sobre este item na coletânea", item.AnotacaoContextual);
    }

    [Fact]
    public void Given_ConteudoColetanea_When_PosicaoAlterada_Then_NovaPosicaoPersistida()
    {
        var item = new ConteudoColetanea
        {
            ColetaneaId = Guid.NewGuid(),
            ConteudoId = Guid.NewGuid(),
            Posicao = 1,
            AnotacaoContextual = null,
            AdicionadoEm = DateTimeOffset.UtcNow
        };

        item.Posicao = 3;

        Assert.Equal(3, item.Posicao);
    }
}
