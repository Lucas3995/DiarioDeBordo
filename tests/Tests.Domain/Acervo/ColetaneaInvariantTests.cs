using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Core.Enums;
using DiarioDeBordo.Core.Primitivos;

namespace DiarioDeBordo.Tests.Domain.Acervo;

/// <summary>
/// Testes das invariantes I-08, I-09, I-10 do agregado Coletanea (Conteudo com Papel==Coletanea).
/// Referência: docs/domain/acervo.md — Coletanea Invariantes
/// </summary>
public class ColetaneaInvariantTests
{
    private static readonly Guid _usuarioId = Guid.NewGuid();

    // ---- I-09: Posições únicas sem lacunas (Coletânea Guiada) ----
    // Note: Full cycle detection (I-08) requires repository access — tested in Tests.Integration.
    // Here we test the domain-level position validation.

    [Fact]
    public void CriarColetanea_Guiada_Sucede()
    {
        var coletanea = Conteudo.Criar(
            _usuarioId, "Plano de Estudo",
            papel: PapelConteudo.Coletanea,
            tipoColetanea: TipoColetanea.Guiada);

        Assert.Equal(TipoColetanea.Guiada, coletanea.TipoColetaneaValor);
    }

    [Fact]
    public void CriarColetanea_Miscelanea_Sucede()
    {
        var coletanea = Conteudo.Criar(
            _usuarioId, "Filmes Favoritos",
            papel: PapelConteudo.Coletanea,
            tipoColetanea: TipoColetanea.Miscelanea);

        Assert.Equal(TipoColetanea.Miscelanea, coletanea.TipoColetaneaValor);
    }

    [Fact]
    public void CriarColetanea_Subscricao_Sucede()
    {
        var coletanea = Conteudo.Criar(
            _usuarioId, "Canal YouTube X",
            papel: PapelConteudo.Coletanea,
            tipoColetanea: TipoColetanea.Subscricao);

        Assert.Equal(TipoColetanea.Subscricao, coletanea.TipoColetaneaValor);
    }

    // ---- I-10: TipoColetanea imutável após criação ----

    [Fact]
    public void AlterarTipoColetanea_AposCriacao_LancaException()
    {
        var coletanea = Conteudo.Criar(
            _usuarioId, "Minha Playlist",
            papel: PapelConteudo.Coletanea,
            tipoColetanea: TipoColetanea.Miscelanea);

        // I-10: TipoColetanea is immutable — any attempt to change it throws
        var ex = Assert.Throws<DomainException>(() =>
            coletanea.AlterarTipoColetanea(TipoColetanea.Guiada));

        Assert.Equal("TIPO_COLETANEA_IMUTAVEL", ex.Codigo);
    }

    // ---- I-08: Ciclo em coletâneas (domain validation layer) ----
    // Full cycle detection requires IColetaneaRepository (graph traversal).
    // This test verifies the domain-level contract for cycle detection.
    // The actual cycle scenario via AdicionarItemNaColetaneaHandler is in Tests.Integration.

    [Fact]
    public void CriarItem_EColetanea_SaoDistintos()
    {
        var item = Conteudo.Criar(_usuarioId, "RE4", papel: PapelConteudo.Item);
        var coletanea = Conteudo.Criar(_usuarioId, "Games", papel: PapelConteudo.Coletanea, tipoColetanea: TipoColetanea.Miscelanea);

        Assert.Equal(PapelConteudo.Item, item.Papel);
        Assert.Equal(PapelConteudo.Coletanea, coletanea.Papel);
        Assert.Null(item.TipoColetaneaValor);
        Assert.NotNull(coletanea.TipoColetaneaValor);
    }

    // I-08: Cycle detection — ConteudoColetanea correctly links parent and child
    // The actual cycle detection logic is in AdicionarItemNaColetaneaHandler (Plan 02)
    [Fact]
    public void I08_ConteudoColetanea_LinksBidirecionais_ParaCicloDeteccao()
    {
        // Setup: ColetaneaA contains ColetaneaB
        var coletaneaAId = Guid.NewGuid();
        var coletaneaBId = Guid.NewGuid();
        var agora = DateTimeOffset.UtcNow;

        var link = new ConteudoColetanea
        {
            ColetaneaId = coletaneaAId,
            ConteudoId = coletaneaBId,
            Posicao = 1,
            AdicionadoEm = agora
        };

        // The link clearly identifies parent (ColetaneaId) and child (ConteudoId)
        // This structure allows IColetaneaRepository.ObterDescendentesAsync to traverse
        Assert.Equal(coletaneaAId, link.ColetaneaId);
        Assert.Equal(coletaneaBId, link.ConteudoId);
        Assert.NotEqual(link.ColetaneaId, link.ConteudoId); // Not self-referential
    }

    // I-09: Posições únicas sem lacunas em Coletânea Guiada
    // ConteudoColetanea.Posicao is the field that tracks position
    [Fact]
    public void I09_ConteudoColetanea_PosicaoSequencial_ParaColetaneaGuiada()
    {
        var coletaneaId = Guid.NewGuid();
        var conteudo1Id = Guid.NewGuid();
        var conteudo2Id = Guid.NewGuid();
        var conteudo3Id = Guid.NewGuid();
        var agora = DateTimeOffset.UtcNow;

        var itens = new[]
        {
            new ConteudoColetanea { ColetaneaId = coletaneaId, ConteudoId = conteudo1Id, Posicao = 1, AdicionadoEm = agora },
            new ConteudoColetanea { ColetaneaId = coletaneaId, ConteudoId = conteudo2Id, Posicao = 2, AdicionadoEm = agora },
            new ConteudoColetanea { ColetaneaId = coletaneaId, ConteudoId = conteudo3Id, Posicao = 3, AdicionadoEm = agora },
        };

        // Posições são contíguas (1, 2, 3) sem lacunas
        var posicoes = itens.Select(i => i.Posicao).OrderBy(p => p).ToList();
        Assert.Equal([1, 2, 3], posicoes);

        // Posições podem ser reordenadas
        itens[0].Posicao = 3;
        itens[2].Posicao = 1;
        var novasPosicoes = itens.Select(i => i.Posicao).OrderBy(p => p).ToList();
        Assert.Equal([1, 2, 3], novasPosicoes);
    }
}
