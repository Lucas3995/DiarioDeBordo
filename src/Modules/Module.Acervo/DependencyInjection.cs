using Microsoft.Extensions.DependencyInjection;

namespace DiarioDeBordo.Module.Acervo;

/// <summary>
/// Extensões de registro de serviços do Module.Acervo.
/// MediatR handlers são auto-registrados via AddMediatR com escaneamento de assembly.
/// </summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "DI wiring — exercised by E2E bootstrap, not unit coverage.")]
public static class AcervoServiceCollectionExtensions
{
    /// <summary>
    /// Registra os serviços do módulo Acervo no container de DI.
    /// Chamado a partir da configuração de DI do projeto Desktop.
    /// </summary>
    /// <param name="services">O container de serviços.</param>
    /// <returns>O mesmo <see cref="IServiceCollection"/> para encadeamento.</returns>
    public static IServiceCollection AddModuleAcervo(this IServiceCollection services)
    {
        // MediatR handlers are auto-registered via AddMediatR scanning (called from host)
        return services;
    }
}
