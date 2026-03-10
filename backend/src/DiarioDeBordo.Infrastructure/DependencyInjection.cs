using DiarioDeBordo.Domain.Auth;
using DiarioDeBordo.Domain.Common;
using DiarioDeBordo.Infrastructure.Auth;
using DiarioDeBordo.Infrastructure.Common;
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
        services.AddSingleton<IClock, SystemClock>();

        // opções fortemente tipadas para DataProtection (criadas manualmente sem depender de pacotes extra)
        var dataProtSection = configuration.GetSection("DataProtection");
        var key = dataProtSection["Key"];
        services.AddSingleton(
            Microsoft.Extensions.Options.Options.Create(new DataProtectionOptions(key ?? string.Empty)));

        services.AddScoped<IDataProtectionService, DataProtectionService>();

        services.AddSingleton<ITokenService>(sp =>
            new TokenService(configuration));

        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();

        return services;
    }
}
