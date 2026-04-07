using System.Diagnostics.CodeAnalysis;

namespace DiarioDeBordo.Module.Acervo.DTOs;

[ExcludeFromCodeCoverage(Justification = "Pure data record")]
public sealed record DuplicataDto(
    Guid ConteudoId,
    string Titulo,
    DateTimeOffset CriadoEm,
    string NivelConfianca);
