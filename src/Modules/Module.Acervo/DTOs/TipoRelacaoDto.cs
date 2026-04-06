namespace DiarioDeBordo.Module.Acervo.DTOs;

/// <summary>DTO de tipo de relação para autocomplete no modal de detalhe.</summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "Pure data record.")]
public sealed record TipoRelacaoDto(Guid Id, string Nome, string NomeInverso, bool IsSistema);
