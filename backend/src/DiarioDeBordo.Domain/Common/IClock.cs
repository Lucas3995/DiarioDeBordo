namespace DiarioDeBordo.Domain.Common;

/// <summary>
/// Abstração do relógio do sistema para obter a data/hora atual em UTC.
/// Permite injeção em testes e evita acoplamento direto a DateTime.UtcNow no domínio.
/// </summary>
public interface IClock
{
    DateTime UtcNow { get; }
}
