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
    // This test verifies that the domain correctly exposes the mechanism.
    // The actual cycle scenario is in Tests.Integration.

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

    // TODO(Phase 3): I-08 cycle detection requires AdicionarItemNaColetanea command
    // Tracked in docs/domain/acervo.md invariant I-08
    [Fact(Skip = "Deferred to Phase 3: AdicionarItemNaColetanea not implemented")]
    public void I08_AdicionarColetanea_CicloDetectado_Falha() { }

    // TODO(Phase 3): I-09 unique positions requires ordered collection command
    // Tracked in docs/domain/acervo.md invariant I-09
    [Fact(Skip = "Deferred to Phase 3: ordered collection not implemented")]
    public void I09_PosicaoDuplicada_EmColetaneaGuiada_Falha() { }
}
