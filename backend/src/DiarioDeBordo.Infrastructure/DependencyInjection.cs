using DiarioDeBordo.Domain.Auth;
using DiarioDeBordo.Domain.Common;
using DiarioDeBordo.Infrastructure.Auth;
using DiarioDeBordo.Infrastructure.Common;
using DiarioDeBordo.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;

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
        if (string.IsNullOrWhiteSpace(key))
        {
            // ambiente de CI ou desenvolvimento sem chave explícita: gerar aleatória e avisar.
            key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            Console.WriteLine("[DI] DataProtection:Key ausente, gerando chave temporária.");
        }
        services.AddSingleton(
            Microsoft.Extensions.Options.Options.Create(new DataProtectionOptions(key)));

        services.AddScoped<IDataProtectionService, DataProtectionService>();

        services.AddSingleton<ITokenService>(sp =>
            new TokenService(configuration));

        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();

        return services;
    }
}
