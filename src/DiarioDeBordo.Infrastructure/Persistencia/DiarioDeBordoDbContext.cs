using DiarioDeBordo.Core.Entidades;
using Microsoft.EntityFrameworkCore;

namespace DiarioDeBordo.Infrastructure.Persistencia;

/// <summary>
/// DbContext do DiarioDeBordo.
/// </summary>
public sealed class DiarioDeBordoDbContext : DbContext
{
    public DiarioDeBordoDbContext(DbContextOptions<DiarioDeBordoDbContext> options)
        : base(options) { }

    public DbSet<Conteudo> Conteudos => Set<Conteudo>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<TipoRelacao> TipoRelacoes => Set<TipoRelacao>();
    public DbSet<Relacao> Relacoes => Set<Relacao>();
    public DbSet<ConteudoCategoria> ConteudoCategorias => Set<ConteudoCategoria>();
    public DbSet<ConteudoColetanea> ConteudoColetaneas => Set<ConteudoColetanea>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        // Apply all IEntityTypeConfiguration<T> from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DiarioDeBordoDbContext).Assembly);
    }
}
