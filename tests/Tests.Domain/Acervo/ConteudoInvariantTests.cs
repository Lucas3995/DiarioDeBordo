using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Core.Enums;
using DiarioDeBordo.Core.Primitivos;

namespace DiarioDeBordo.Tests.Domain.Acervo;

/// <summary>
/// Testes das invariantes I-01 a I-07 do agregado Conteudo.
/// Executam sem banco de dados — apenas lógica de domínio.
/// Referência: docs/domain/acervo.md — Mapeamento de Invariantes → Testes
/// </summary>
public class ConteudoInvariantTests
{
    private static readonly Guid _usuarioId = Guid.NewGuid();

    // ---- I-01: Título obrigatório ----

    [Fact]
    public void CriarConteudo_TituloVazio_LancaException()
    {
        var ex = Assert.Throws<DomainException>(() =>
            Conteudo.Criar(_usuarioId, ""));

        Assert.Equal("TITULO_OBRIGATORIO", ex.Codigo);
    }

    [Fact]
    public void CriarConteudo_TituloSomenteEspacos_LancaException()
    {
        var ex = Assert.Throws<DomainException>(() =>
            Conteudo.Criar(_usuarioId, "   "));

        Assert.Equal("TITULO_OBRIGATORIO", ex.Codigo);
    }

    [Fact]
    public void CriarConteudo_TituloValido_Sucede()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "Dune");

        Assert.Equal("Dune", conteudo.Titulo);
        Assert.Equal(_usuarioId, conteudo.UsuarioId);
        Assert.NotEqual(Guid.Empty, conteudo.Id);
    }

    [Fact]
    public void CriarConteudo_TituloComEspacosNasExtremidades_EhTrimado()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "  Dune  ");

        Assert.Equal("Dune", conteudo.Titulo);
    }

    // ---- I-02: TipoColetanea nulo quando Papel==Item; obrigatório quando Papel==Coletanea ----

    [Fact]
    public void CriarColetanea_SemTipoColetanea_LancaException()
    {
        var ex = Assert.Throws<DomainException>(() =>
            Conteudo.Criar(_usuarioId, "Minha Playlist",
                papel: PapelConteudo.Coletanea,
                tipoColetanea: null));

        Assert.Equal("TIPO_COLETANEA_OBRIGATORIO", ex.Codigo);
    }

    [Fact]
    public void CriarColetanea_ComTipoColetanea_Sucede()
    {
        var coletanea = Conteudo.Criar(_usuarioId, "Minha Playlist",
            papel: PapelConteudo.Coletanea,
            tipoColetanea: TipoColetanea.Miscelanea);

        Assert.Equal(PapelConteudo.Coletanea, coletanea.Papel);
        Assert.Equal(TipoColetanea.Miscelanea, coletanea.TipoColetaneaValor);
    }

    [Fact]
    public void CriarItem_ComTipoColetanea_LancaException()
    {
        // Item não pode ter TipoColetanea
        var ex = Assert.Throws<DomainException>(() =>
            Conteudo.Criar(_usuarioId, "Dune",
                papel: PapelConteudo.Item,
                tipoColetanea: TipoColetanea.Guiada));

        Assert.Equal("TIPO_COLETANEA_DEVE_SER_NULO", ex.Codigo);
    }

    // ---- I-03: Nota no intervalo [0, 10] ----

    [Theory]
    [InlineData(11)]
    [InlineData(10.1)]
    [InlineData(-0.1)]
    [InlineData(-1)]
    public void DefinirNota_ForaDaFaixa_LancaException(decimal nota)
    {
        var conteudo = Conteudo.Criar(_usuarioId, "Dune");

        var ex = Assert.Throws<DomainException>(() => conteudo.DefinirNota(nota));

        Assert.Equal("NOTA_FORA_DA_FAIXA", ex.Codigo);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(7.5)]
    public void DefinirNota_DentroFaixa_Sucede(decimal nota)
    {
        var conteudo = Conteudo.Criar(_usuarioId, "Dune");

        conteudo.DefinirNota(nota);

        Assert.Equal(nota, conteudo.Nota);
    }

    // ---- I-04: Máximo 20 imagens por conteúdo ----

    [Fact]
    public void AdicionarImagem_AcimaDe20_LancaException()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "Caderno de Desenhos");

        // Add 20 images (allowed)
        for (var i = 0; i < 20; i++)
            conteudo.AdicionarImagem($"/path/img{i}.jpg", OrigemImagem.Manual, tamanhoBytes: 1024);

        // 21st image must fail
        var ex = Assert.Throws<DomainException>(() =>
            conteudo.AdicionarImagem("/path/img21.jpg", OrigemImagem.Manual, tamanhoBytes: 1024));

        Assert.Equal("LIMITE_IMAGENS_ATINGIDO", ex.Codigo);
    }

    // ---- I-05: Máximo 10 MB por imagem ----

    [Fact]
    public void AdicionarImagem_Acima10MB_LancaException()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "Caderno");
        const long excedente = 10L * 1024 * 1024 + 1; // 10MB + 1 byte

        var ex = Assert.Throws<DomainException>(() =>
            conteudo.AdicionarImagem("/path/img.jpg", OrigemImagem.Manual, tamanhoBytes: excedente));

        Assert.Equal("IMAGEM_EXCEDE_TAMANHO", ex.Codigo);
    }

    [Fact]
    public void AdicionarImagem_Exatamente10MB_Sucede()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "Caderno");
        const long exato = 10L * 1024 * 1024; // exactly 10MB

        conteudo.AdicionarImagem("/path/img.jpg", OrigemImagem.Manual, tamanhoBytes: exato);

        Assert.Single(conteudo.Imagens);
    }

    // ---- I-06: Apenas uma imagem principal ----

    [Fact]
    public void PrimeiraImagem_EhPrincipalAutomaticamente()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "Caderno");

        conteudo.AdicionarImagem("/path/img1.jpg", OrigemImagem.Manual, tamanhoBytes: 1024);

        Assert.True(conteudo.Imagens[0].Principal);
    }

    [Fact]
    public void DefinirImagemPrincipal_DesativaPreviaPrincipal()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "Caderno");

        conteudo.AdicionarImagem("/path/img1.jpg", OrigemImagem.Manual, tamanhoBytes: 1024);
        conteudo.AdicionarImagem("/path/img2.jpg", OrigemImagem.Manual, tamanhoBytes: 1024);

        // img1 is principal by default; change to img2
        var img2Id = conteudo.Imagens[1].Id;
        conteudo.DefinirImagemPrincipal(img2Id);

        Assert.False(conteudo.Imagens[0].Principal); // img1 no longer principal
        Assert.True(conteudo.Imagens[1].Principal);  // img2 is principal
    }

    [Fact]
    public void DefinirImagemPrincipal_ImagemInexistente_LancaException()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "Caderno");
        conteudo.AdicionarImagem("/path/img1.jpg", OrigemImagem.Manual, tamanhoBytes: 1024);

        var ex = Assert.Throws<DomainException>(() =>
            conteudo.DefinirImagemPrincipal(Guid.NewGuid())); // non-existent ID

        Assert.Equal("NAO_ENCONTRADO", ex.Codigo);
    }

    // ---- I-07: Prioridade de fonte única por conteúdo ----

    [Fact]
    public void AdicionarFonte_PrioridadeDuplicada_LancaException()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "RE4");
        conteudo.AdicionarFonte(TipoFonte.ArquivoLocal, "/games/re4.iso", prioridade: 1);

        var ex = Assert.Throws<DomainException>(() =>
            conteudo.AdicionarFonte(TipoFonte.Url, "https://steam.com/re4", prioridade: 1));

        Assert.Equal("PRIORIDADE_FONTE_DUPLICADA", ex.Codigo);
    }

    [Fact]
    public void AdicionarFontes_PrioridadesDistintas_Sucede()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "RE4");

        conteudo.AdicionarFonte(TipoFonte.ArquivoLocal, "/games/re4.iso", prioridade: 1);
        conteudo.AdicionarFonte(TipoFonte.Url, "https://steam.com/re4", prioridade: 2);
        conteudo.AdicionarFonte(TipoFonte.Url, "https://gog.com/re4", prioridade: 3);

        Assert.Equal(3, conteudo.Fontes.Count);
    }
}
