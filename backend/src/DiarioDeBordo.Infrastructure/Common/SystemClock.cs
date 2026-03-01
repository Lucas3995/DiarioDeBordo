using DiarioDeBordo.Domain.Common;

namespace DiarioDeBordo.Infrastructure.Common;

/// <summary>
/// Implementação de IClock que retorna DateTime.UtcNow.
/// Registrada no DI na camada de infraestrutura.
/// </summary>
public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
