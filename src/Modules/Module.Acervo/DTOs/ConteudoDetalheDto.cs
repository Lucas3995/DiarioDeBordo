namespace DiarioDeBordo.Module.Acervo.DTOs;

/// <summary>DTO de detalhe — usado na tela de detalhe do conteúdo.</summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "Pure data record — no business logic; compiler-generated members (Equals/Deconstruct) skew coverage.")]
public sealed record ConteudoDetalheDto(
    Guid Id,
    string Titulo,
    string? Descricao,
    string? Anotacoes,
    decimal? Nota,
    string Formato,
    string Papel,
    DateTimeOffset CriadoEm);
