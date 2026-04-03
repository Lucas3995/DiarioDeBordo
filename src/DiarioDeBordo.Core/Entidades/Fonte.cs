using DiarioDeBordo.Core.Enums;

namespace DiarioDeBordo.Core.Entidades;

public sealed class Fonte
{
    public Guid Id { get; init; }
    public Guid ConteudoId { get; init; }
    public TipoFonte Tipo { get; init; }
    public required string Valor { get; init; }
    public string? Plataforma { get; init; }
    public int Prioridade { get; init; }
}
