using System.Diagnostics.CodeAnalysis;

namespace DiarioDeBordo.Module.Acervo.DTOs;

[ExcludeFromCodeCoverage(Justification = "Pure data record")]
public sealed record ImagemDto(
    Guid Id,
    string Caminho,
    string Origem,
    bool Principal);
