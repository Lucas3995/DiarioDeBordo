using System.Diagnostics.CodeAnalysis;

namespace DiarioDeBordo.Module.Acervo.DTOs;

[ExcludeFromCodeCoverage(Justification = "Pure data record")]
public sealed record FonteDto(
    Guid Id,
    string Tipo,
    string Valor,
    string? Plataforma,
    int Prioridade);
