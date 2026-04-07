using DiarioDeBordo.Core.Consultas;
using DiarioDeBordo.Core.Repositorios;
using DiarioDeBordo.Infrastructure.Consultas;
using DiarioDeBordo.Infrastructure.Persistencia;
using DiarioDeBordo.Infrastructure.Repositorios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace DiarioDeBordo.Infrastructure;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "DI wiring — validated end-to-end by integration and E2E tests, not by unit coverage.")]
public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<DiarioDeBordoDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Repositories
        services.AddScoped<IConteudoRepository, ConteudoRepository>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<ITipoRelacaoRepository, TipoRelacaoRepository>();
        services.AddScoped<IRelacaoRepository, RelacaoRepository>();
        services.AddScoped<IColetaneaRepository, ColetaneaRepository>();

        // Query services (CQRS read-side)
        services.AddScoped<IConteudoQueryService, ConteudoQueryService>();
        services.AddScoped<IColetaneaQueryService, ColetaneaQueryService>();
        services.AddScoped<IDeduplicacaoService, DeduplicacaoService>();

        return services;
    }
}
