using DiarioDeBordo.Core.Infraestrutura;
using DiarioDeBordo.Infrastructure.Postgres;
using DiarioDeBordo.Infrastructure.Seguranca;
using Microsoft.Extensions.DependencyInjection;

namespace DiarioDeBordo.Infrastructure;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "Pre-DI bootstrap wiring — executes only during app startup, validated by E2E smoke test.")]
public static class DependencyInjectionBootstrap
{
    /// <summary>
    /// Registers only the services needed for the pre-DI bootstrap phase:
    /// secure storage and PostgresBootstrap.
    /// Call this on a minimal ServiceCollection BEFORE AddInfrastructure.
    /// Used by App.axaml.cs to get the real connection string before building the full DI container.
    /// </summary>
    public static IServiceCollection AddInfrastructureBootstrap(this IServiceCollection services)
    {
        // OS-specific secure storage registration — guarded at call site to satisfy CA1416
        if (OperatingSystem.IsWindows())
        {
#pragma warning disable CA1416 // Validated by OperatingSystem.IsWindows() check above
            services.AddSingleton<IArmazenamentoSeguro, ArmazenamentoSeguroWindows>();
#pragma warning restore CA1416
        }
        else
        {
#pragma warning disable CA1416 // Non-Windows defaults to Linux implementation; macOS not supported
            services.AddSingleton<IArmazenamentoSeguro, ArmazenamentoSeguroLinux>();
#pragma warning restore CA1416
        }

        services.AddSingleton<IPostgresBootstrap, PostgresBootstrap>();

        return services;
    }
}
