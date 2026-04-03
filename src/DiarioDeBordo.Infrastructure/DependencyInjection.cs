using DiarioDeBordo.Core.Consultas;
using DiarioDeBordo.Core.Repositorios;
using DiarioDeBordo.Infrastructure.Consultas;
using DiarioDeBordo.Infrastructure.Persistencia;
using DiarioDeBordo.Infrastructure.Repositorios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DiarioDeBordo.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<DiarioDeBordoDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IConteudoRepository, ConteudoRepository>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<IConteudoQueryService, ConteudoQueryService>();

        return services;
    }
}
