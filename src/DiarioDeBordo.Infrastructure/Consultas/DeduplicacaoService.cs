using System.Globalization;
using System.Text;
using DiarioDeBordo.Core.Consultas;
using DiarioDeBordo.Core.Enums;
using DiarioDeBordo.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace DiarioDeBordo.Infrastructure.Consultas;

/// <summary>
/// Serviço de detecção de duplicatas com dois níveis de confiança (D-07).
/// Alta: URL exata em qualquer fonte. Media: título normalizado.
/// REGRA: toda query filtra por usuarioId — nunca acessa dados de outro usuário (SEG-02).
/// </summary>
internal sealed class DeduplicacaoService : IDeduplicacaoService
{
    private readonly DiarioDeBordoDbContext _context;

    public DeduplicacaoService(DiarioDeBordoDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Normalizes title for comparison: trim, lowercase, remove diacritics and punctuation, collapse whitespace.
    /// Public static for testability.
    /// </summary>
    public static string NormalizarTitulo(string titulo)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            return string.Empty;

        // Trim and lowercase
        var normalized = titulo.Trim().ToLowerInvariant();

        // Remove diacritics (accents): Normalize to FormD, filter NonSpacingMark, Normalize to FormC
        normalized = RemoverDiacriticos(normalized);

        // Remove punctuation
        var sb = new StringBuilder(normalized.Length);
        foreach (var c in normalized)
        {
            if (!char.IsPunctuation(c))
                sb.Append(c);
        }
        normalized = sb.ToString();

        // Collapse whitespace: split by default separators, remove empty, rejoin with single space
        var words = normalized.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        return string.Join(' ', words);
    }

    private static string RemoverDiacriticos(string texto)
    {
        var normalizedString = texto.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(normalizedString.Length);

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    public async Task<DuplicataData?> VerificarAsync(
        Guid usuarioId, string titulo, IReadOnlyList<string>? fonteUrls, CancellationToken ct)
    {
        // Level 1: HIGH confidence — exact URL match in any fonte
        if (fonteUrls is { Count: > 0 })
        {
            var duplicataUrl = await _context.Conteudos
                .Where(c => c.UsuarioId == usuarioId) // SEG-02
                .Where(c => c.Fontes.Any(f => fonteUrls.Contains(f.Valor)))
                .Select(c => new DuplicataData(c.Id, c.Titulo, c.CriadoEm, NivelConfiancaDuplicata.Alta))
                .FirstOrDefaultAsync(ct)
                .ConfigureAwait(false);

            if (duplicataUrl is not null)
                return duplicataUrl;
        }

        // Level 2: MEDIUM confidence — normalized title match
        // For Phase 4 scale (<10k items per user per RESEARCH.md Pitfall 6),
        // we can normalize in-memory after a case-insensitive pre-filter.
        var tituloNormalizado = NormalizarTitulo(titulo);
        if (string.IsNullOrEmpty(tituloNormalizado))
            return null;

        // Pre-filter: use original trimmed title prefix (not normalized) to avoid
        // accent-related false negatives in ILike against raw DB title.
        var tituloOriginal = titulo.Trim();
        var prefixo = tituloOriginal.Length >= 3
            ? tituloOriginal[..3]
            : tituloOriginal;

        var candidatos = await _context.Conteudos
            .Where(c => c.UsuarioId == usuarioId) // SEG-02
            .Where(c => EF.Functions.ILike(c.Titulo, $"{prefixo}%"))
            .Select(c => new { c.Id, c.Titulo, c.CriadoEm })
            .ToListAsync(ct)
            .ConfigureAwait(false);

        foreach (var candidato in candidatos)
        {
            var candidatoNormalizado = NormalizarTitulo(candidato.Titulo);
            if (string.Equals(tituloNormalizado, candidatoNormalizado, StringComparison.Ordinal))
            {
                return new DuplicataData(
                    candidato.Id, candidato.Titulo, candidato.CriadoEm, NivelConfiancaDuplicata.Media);
            }
        }

        return null;
    }
}
