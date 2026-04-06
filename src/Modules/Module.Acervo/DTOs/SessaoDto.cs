using DiarioDeBordo.Core.Enums;

namespace DiarioDeBordo.Module.Acervo.DTOs;

/// <summary>DTO de sessão de consumo (conteúdo filho) para exibição na timeline (D-18).</summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "Pure data record.")]
public sealed record SessaoDto(
    Guid Id,
    string Titulo,
    DateTimeOffset CriadoEm,
    Classificacao? Classificacao,
    decimal? Nota,
    string? Anotacoes);
