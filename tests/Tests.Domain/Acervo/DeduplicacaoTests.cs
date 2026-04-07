using System.Globalization;
using System.Text;

namespace DiarioDeBordo.Tests.Domain.Acervo;

/// <summary>
/// Testes de deduplicação de conteúdo (D-07).
/// Wave 0: normalização de título implementada; IDeduplicacaoService stubs para Plan 02.
/// </summary>
public class DeduplicacaoTests
{
    /// <summary>
    /// Normaliza título para detecção de duplicatas de média confiança.
    /// Remove espaços, converte para maiúsculo (CA1308), remove diacríticos e pontuação.
    /// </summary>
    private static string NormalizarTitulo(string titulo)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            return string.Empty;

        // 1. Trim e uppercase (CA1308 compliance)
        var normalizado = titulo.Trim().ToUpperInvariant();

        // 2. Remove diacríticos (accents)
        normalizado = RemoverDiacriticos(normalizado);

        // 3. Remove pontuação e caracteres especiais (keep letters, digits, spaces)
        var sb = new StringBuilder(normalizado.Length);
        foreach (var c in normalizado)
        {
            if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
                sb.Append(c);
        }

        return sb.ToString();
    }

    private static string RemoverDiacriticos(string texto)
    {
        var normalizedString = texto.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(normalizedString.Length);

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    [Theory]
    [InlineData("  Duna  ", "DUNA")]
    [InlineData("DUNA", "DUNA")]
    [InlineData("Duna: Crônicas", "DUNA CRONICAS")]
    [InlineData("Duna — Frank Herbert!", "DUNA  FRANK HERBERT")]
    [InlineData("Café com Leite", "CAFE COM LEITE")]
    [InlineData("São Paulo", "SAO PAULO")]
    [InlineData("Über", "UBER")]
    public void Given_TituloComVariacoes_When_Normalizado_Then_FormaCanonica(string input, string expected)
    {
        var resultado = NormalizarTitulo(input);

        Assert.Equal(expected, resultado);
    }

    [Fact(Skip = "Wave 0 stub — implementation in Plan 02")]
    public void Given_UrlIdentica_When_VerificarDuplicata_Then_RetornaAltaConfianca()
    {
        // TODO: Implement when IDeduplicacaoService is implemented
        // Test that exact URL match returns NivelConfiancaDuplicata.Alta
    }

    [Fact(Skip = "Wave 0 stub — implementation in Plan 02")]
    public void Given_TituloSemelhante_When_VerificarDuplicata_Then_RetornaMediaConfianca()
    {
        // TODO: Implement when IDeduplicacaoService is implemented
        // Test that normalized title match returns NivelConfiancaDuplicata.Media
    }

    [Fact(Skip = "Wave 0 stub — implementation in Plan 02")]
    public void Given_NenhumMatch_When_VerificarDuplicata_Then_RetornaNull()
    {
        // TODO: Implement when IDeduplicacaoService is implemented
        // Test that no match returns null
    }
}
