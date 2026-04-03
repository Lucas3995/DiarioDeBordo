using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DiarioDeBordo.Infrastructure.Persistencia;

/// <summary>
/// Usado pelo CLI do EF Core em design time (dotnet ef migrations add).
/// Não é usado em runtime — a conexão real vem do DI container.
/// Set DIARIODEBORDO_EF_DEV_PG_PASSWORD before running migrations.
/// </summary>
internal sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DiarioDeBordoDbContext>
{
    public DiarioDeBordoDbContext CreateDbContext(string[] args)
    {
        // Falls back to a placeholder so `dotnet ef migrations add` works without a live DB.
        // The actual connection string is injected at runtime via DI.
        var password = Environment.GetEnvironmentVariable("DIARIODEBORDO_EF_DEV_PG_PASSWORD")
            ?? "design_time_placeholder";

        var optionsBuilder = new DbContextOptionsBuilder<DiarioDeBordoDbContext>();
        optionsBuilder.UseNpgsql(
            $"Host=localhost;Port=15432;Database=diariodebordo_dev;Username=postgres;Password={password}");

        return new DiarioDeBordoDbContext(optionsBuilder.Options);
    }
}
