namespace DiarioDeBordo.Module.Acervo.DTOs;

/// <summary>DTO de relação entre conteúdos.</summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "Pure data record.")]
public sealed record RelacaoDto(
    Guid Id,
    Guid ConteudoDestinoId,
    string TituloDestino,
    string NomeTipo,
    bool IsInversa);
