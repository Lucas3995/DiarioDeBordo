using System.Diagnostics.CodeAnalysis;

namespace DiarioDeBordo.Module.Acervo.DTOs;

[ExcludeFromCodeCoverage(Justification = "Pure data record")]
public sealed record ColetaneaItemDto(
    Guid ConteudoId,
    string Titulo,
    int Posicao,
    string? AnotacaoContextual,
    string EstadoProgresso,
    string Papel,
    string? TipoColetanea,
    int? SubItensContagem,
    DateTimeOffset AdicionadoEm);
