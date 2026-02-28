using DiarioDeBordo.Application.Auth;
using DiarioDeBordo.Domain.Interfaces;
using DiarioDeBordo.Infrastructure.Auth;
using DiarioDeBordo.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiarioDeBordo.Infrastructure;

/// <summary>
/// Registra serviços de infraestrutura no contêiner de DI.
/// Implementações concretas de interfaces definidas no Domain e Application.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IDataProtectionService, DataProtectionService>();

        services.AddSingleton<ITokenService>(sp =>
            new TokenService(configuration));

        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();

        return services;
    }
}
