namespace DiarioDeBordo.Module.Acervo.DTOs;

/// <summary>DTO de categoria associada a um conteúdo.</summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "Pure data record.")]
public sealed record CategoriaDto(Guid Id, string Nome, bool IsAutomatica);
