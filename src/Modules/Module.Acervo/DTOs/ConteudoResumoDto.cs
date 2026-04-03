namespace DiarioDeBordo.Module.Acervo.DTOs;

/// <summary>DTO de resumo — usado na listagem de conteúdos (list view).</summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "Pure data record — no business logic; compiler-generated members (Equals/Deconstruct) skew coverage.")]
public sealed record ConteudoResumoDto(
    Guid Id,
    string Titulo,
    string Formato,
    string Papel,
    DateTimeOffset CriadoEm);
