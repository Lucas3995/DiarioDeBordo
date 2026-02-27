using DiarioDeBordo.Domain.Interfaces;
using DiarioDeBordo.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;

namespace DiarioDeBordo.Infrastructure;

/// <summary>
/// Registra serviços de infraestrutura no contêiner de DI.
/// Implementações concretas de interfaces definidas no Domain.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IDataProtectionService, DataProtectionService>();

        return services;
    }
}
