namespace DiarioDeBordo.Application.Status;

/// <summary>
/// Abstração para obter o nome do ambiente de execução (Development, Production, etc.).
/// Permite que a camada Application não dependa de IHostEnvironment.
/// </summary>
public interface IAmbiente
{
    string NomeAmbiente { get; }
}
