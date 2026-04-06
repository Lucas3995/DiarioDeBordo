using DiarioDeBordo.Core.Enums;

namespace DiarioDeBordo.Core.Entidades;

public sealed record Progresso
{
    public EstadoProgresso Estado { get; init; } = EstadoProgresso.NaoIniciado;
    public string? PosicaoAtual { get; init; }
    public string? NotaManual { get; init; }
}
