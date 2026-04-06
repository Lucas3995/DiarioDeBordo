using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Core.Enums;

namespace DiarioDeBordo.Tests.Domain.Acervo;

public class AlterarProgressoTests
{
    private static readonly Guid _usuarioId = Guid.NewGuid();

    [Fact]
    public void ProgressoInicial_EhNaoIniciado()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "Dune");

        Assert.Equal(EstadoProgresso.NaoIniciado, conteudo.Progresso.Estado);
        Assert.Null(conteudo.Progresso.PosicaoAtual);
    }

    [Fact]
    public void AlterarProgresso_EmAndamento_ComPosicao()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "Dune");

        conteudo.AlterarProgresso(EstadoProgresso.EmAndamento, posicaoAtual: "Capítulo 5");

        Assert.Equal(EstadoProgresso.EmAndamento, conteudo.Progresso.Estado);
        Assert.Equal("Capítulo 5", conteudo.Progresso.PosicaoAtual);
    }

    [Fact]
    public void AlterarProgresso_Concluido_LimpaposicaoSeNaoInformada()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "Dune");
        conteudo.AlterarProgresso(EstadoProgresso.EmAndamento, "Capítulo 10");

        conteudo.AlterarProgresso(EstadoProgresso.Concluido);

        Assert.Equal(EstadoProgresso.Concluido, conteudo.Progresso.Estado);
    }
}
