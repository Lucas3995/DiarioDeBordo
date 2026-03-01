using DiarioDeBordo.Application.Status;
using Microsoft.Extensions.Hosting;

namespace DiarioDeBordo.Api.Composition;

/// <summary>
/// Implementação de IAmbiente que lê o nome do ambiente do host (Development, Production, etc.).
/// </summary>
public sealed class HostAmbienteAdapter(IHostEnvironment hostEnvironment) : IAmbiente
{
    public string NomeAmbiente => hostEnvironment.EnvironmentName;
}
