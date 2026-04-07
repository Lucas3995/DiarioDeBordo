using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Core.Enums;
using DiarioDeBordo.Core.Primitivos;

namespace DiarioDeBordo.Tests.Domain.Acervo;

/// <summary>
/// Testes de gerenciamento de fontes no agregado Conteudo.
/// ACE-06: Fontes com prioridade e fallback.
/// </summary>
public class FonteManagementTests
{
    private static readonly Guid _usuarioId = Guid.NewGuid();

    [Fact]
    public void Given_ConteudoComFontes_When_RemoverFonteValida_Then_FonteRemovida()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "RE4");
        conteudo.AdicionarFonte(TipoFonte.ArquivoLocal, "/games/re4.iso", prioridade: 1);
        conteudo.AdicionarFonte(TipoFonte.Url, "https://steam.com/re4", prioridade: 2);

        var fonteIdRemover = conteudo.Fontes[0].Id;

        conteudo.RemoverFonte(fonteIdRemover);

        Assert.Single(conteudo.Fontes);
        Assert.DoesNotContain(conteudo.Fontes, f => f.Id == fonteIdRemover);
    }

    [Fact]
    public void Given_ConteudoComFontes_When_RemoverFonteInexistente_Then_ThrowsDomainException()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "RE4");
        conteudo.AdicionarFonte(TipoFonte.ArquivoLocal, "/games/re4.iso", prioridade: 1);

        var ex = Assert.Throws<DomainException>(() =>
            conteudo.RemoverFonte(Guid.NewGuid()));

        Assert.Equal("NAO_ENCONTRADO", ex.Codigo);
    }

    [Fact]
    public void Given_ConteudoComTresFontes_When_ReordenarFontes_Then_PrioridadesReassignadas()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "RE4");
        conteudo.AdicionarFonte(TipoFonte.ArquivoLocal, "/games/re4.iso", prioridade: 1);
        conteudo.AdicionarFonte(TipoFonte.Url, "https://steam.com/re4", prioridade: 2);
        conteudo.AdicionarFonte(TipoFonte.Url, "https://gog.com/re4", prioridade: 3);

        var fonte1Id = conteudo.Fontes[0].Id;
        var fonte2Id = conteudo.Fontes[1].Id;
        var fonte3Id = conteudo.Fontes[2].Id;

        // Reorder: 3rd becomes 1st, 1st becomes 2nd, 2nd becomes 3rd
        conteudo.ReordenarFontes([fonte3Id, fonte1Id, fonte2Id]);

        var fonteAntigamente3 = conteudo.Fontes.First(f => f.Id == fonte3Id);
        var fonteAntigamente1 = conteudo.Fontes.First(f => f.Id == fonte1Id);
        var fonteAntigamente2 = conteudo.Fontes.First(f => f.Id == fonte2Id);

        Assert.Equal(1, fonteAntigamente3.Prioridade);
        Assert.Equal(2, fonteAntigamente1.Prioridade);
        Assert.Equal(3, fonteAntigamente2.Prioridade);
    }

    [Fact]
    public void Given_ConteudoComFontes_When_ReordenarComIdsInconsistentes_Then_ThrowsDomainException()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "RE4");
        conteudo.AdicionarFonte(TipoFonte.ArquivoLocal, "/games/re4.iso", prioridade: 1);
        conteudo.AdicionarFonte(TipoFonte.Url, "https://steam.com/re4", prioridade: 2);

        // Try to reorder with wrong IDs
        var ex = Assert.Throws<DomainException>(() =>
            conteudo.ReordenarFontes([Guid.NewGuid(), Guid.NewGuid()]));

        Assert.Equal("FONTES_INCONSISTENTES", ex.Codigo);
    }

    [Fact]
    public void Given_ConteudoComFontes_When_ReordenarComListaIncompleta_Then_ThrowsDomainException()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "RE4");
        conteudo.AdicionarFonte(TipoFonte.ArquivoLocal, "/games/re4.iso", prioridade: 1);
        conteudo.AdicionarFonte(TipoFonte.Url, "https://steam.com/re4", prioridade: 2);

        var fonte1Id = conteudo.Fontes[0].Id;
        // Try to reorder with only one of the two IDs
        var ex = Assert.Throws<DomainException>(() =>
            conteudo.ReordenarFontes([fonte1Id]));

        Assert.Equal("FONTES_INCONSISTENTES", ex.Codigo);
    }

    [Fact]
    public void Given_ConteudoComImagem_When_RemoverImagemValida_Then_ImagemRemovida()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "Caderno");
        conteudo.AdicionarImagem("/path/img.jpg", OrigemImagem.Manual, tamanhoBytes: 1024);

        var imagemId = conteudo.Imagens[0].Id;

        conteudo.RemoverImagem(imagemId);

        Assert.Empty(conteudo.Imagens);
    }

    [Fact]
    public void Given_ConteudoComImagem_When_RemoverImagemInexistente_Then_ThrowsDomainException()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "Caderno");
        conteudo.AdicionarImagem("/path/img.jpg", OrigemImagem.Manual, tamanhoBytes: 1024);

        var ex = Assert.Throws<DomainException>(() =>
            conteudo.RemoverImagem(Guid.NewGuid()));

        Assert.Equal("NAO_ENCONTRADO", ex.Codigo);
    }

    [Fact]
    public void Given_ConteudoComMultiplasImagens_When_RemoverPrimeira_Then_OutrasPermanecem()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "Caderno");
        conteudo.AdicionarImagem("/path/img1.jpg", OrigemImagem.Manual, tamanhoBytes: 1024);
        conteudo.AdicionarImagem("/path/img2.jpg", OrigemImagem.Manual, tamanhoBytes: 1024);

        var img1Id = conteudo.Imagens[0].Id;
        var img2Id = conteudo.Imagens[1].Id;

        conteudo.RemoverImagem(img1Id);

        Assert.Single(conteudo.Imagens);
        Assert.Equal(img2Id, conteudo.Imagens[0].Id);
    }
}
