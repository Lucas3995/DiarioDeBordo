using System.Diagnostics.CodeAnalysis;

namespace DiarioDeBordo.Module.Acervo.DTOs;

[ExcludeFromCodeCoverage(Justification = "Pure data record")]
public sealed record ColetaneaDetalheDto(
    Guid Id,
    string Titulo,
    string TipoColetanea,
    int QuantidadeItens,
    decimal ProgressoPercentual,
    string? ImagemCapaCaminho,
    DateTimeOffset CriadoEm);
