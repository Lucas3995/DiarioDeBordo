using DiarioDeBordo.Core.Entidades;
using Microsoft.EntityFrameworkCore;

namespace DiarioDeBordo.Infrastructure.Persistencia;

/// <summary>
/// DbContext do DiarioDeBordo.
/// REGRA: apenas Conteudo e Categoria têm DbSet na Fase 2. Dashboard e Feed são views — nunca persistidas.
/// </summary>
public sealed class DiarioDeBordoDbContext : DbContext
{
    public DiarioDeBordoDbContext(DbContextOptions<DiarioDeBordoDbContext> options)
        : base(options) { }

    public DbSet<Conteudo> Conteudos => Set<Conteudo>();
    public DbSet<Categoria> Categorias => Set<Categoria>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        // Apply all IEntityTypeConfiguration<T> from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DiarioDeBordoDbContext).Assembly);
    }
}
