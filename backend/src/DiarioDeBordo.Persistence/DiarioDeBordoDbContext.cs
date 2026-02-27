using DiarioDeBordo.Domain.Common;
using DiarioDeBordo.Domain.Interfaces;
using DiarioDeBordo.Domain.Obras;
using Microsoft.EntityFrameworkCore;

namespace DiarioDeBordo.Persistence;

/// <summary>
/// DbContext principal da aplicação. Responsável por toda a configuração
/// de mapeamento EF Core e aplicação de auditoria automática.
/// 
/// Conformidade LGPD: conexões via TLS (configurado na connection string);
/// entidades de dados pessoais serão marcadas nos respectivos comentários de configuração.
/// </summary>
public class DiarioDeBordoDbContext(DbContextOptions<DiarioDeBordoDbContext> options)
    : DbContext(options), IUnitOfWork
{
    public DbSet<Obra> Obras => Set<Obra>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DiarioDeBordoDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AtualizarAuditoria();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> SalvarAlteracoesAsync(CancellationToken cancellationToken = default)
        => await SaveChangesAsync(cancellationToken);

    private void AtualizarAuditoria()
    {
        var entradas = ChangeTracker.Entries<AuditableEntity>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified);

        foreach (var entrada in entradas)
        {
            if (entrada.State == EntityState.Added)
                entrada.Property(nameof(AuditableEntity.CriadoEm)).CurrentValue = DateTime.UtcNow;

            entrada.Property(nameof(AuditableEntity.AtualizadoEm)).CurrentValue = DateTime.UtcNow;
        }
    }
}
