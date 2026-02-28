using DiarioDeBordo.Application.Auth;
using DiarioDeBordo.Application.Obras.Listar;
using DiarioDeBordo.Domain.Interfaces;
using DiarioDeBordo.Persistence.Auth;
using DiarioDeBordo.Persistence.Obras;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiarioDeBordo.Persistence;

/// <summary>
/// Registra serviços de persistência no contêiner de DI.
/// Connection string lida de ConnectionStrings:DefaultConnection (variável de ambiente
/// ou secrets — nunca hardcodada conforme regra DevSecOps).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' não configurada. " +
                "Defina via variável de ambiente ConnectionStrings__DefaultConnection.");

        services.AddDbContext<DiarioDeBordoDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsAssembly(typeof(DiarioDeBordoDbContext).Assembly.FullName)));

        services.AddScoped<IUnitOfWork>(sp =>
            sp.GetRequiredService<DiarioDeBordoDbContext>());

        services.AddScoped<IObraLeituraRepository, ObraLeituraRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();

        return services;
    }
}
