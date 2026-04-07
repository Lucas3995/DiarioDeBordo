using DiarioDeBordo.Core.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiarioDeBordo.Infrastructure.Persistencia.Configuracoes;

/// <summary>
/// EF Core mapping for ConteudoColetanea join table.
/// NO navigation properties on ConteudoColetanea (per RESEARCH.md Pitfall 1 — EF Core gets confused
/// with self-referencing many-to-many).
/// </summary>
internal sealed class ConteudoColetaneaConfiguration : IEntityTypeConfiguration<ConteudoColetanea>
{
    public void Configure(EntityTypeBuilder<ConteudoColetanea> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("conteudo_coletanea");
        builder.HasKey(cc => new { cc.ColetaneaId, cc.ConteudoId });

        builder.Property(cc => cc.ColetaneaId).HasColumnName("coletanea_id");
        builder.Property(cc => cc.ConteudoId).HasColumnName("conteudo_id");
        builder.Property(cc => cc.Posicao).HasColumnName("posicao");
        builder.Property(cc => cc.AnotacaoContextual)
            .HasColumnName("anotacao_contextual")
            .HasMaxLength(10000);
        builder.Property(cc => cc.AdicionadoEm).HasColumnName("adicionado_em");

        // Collection FK: cascade delete (removing collection removes associations)
        builder.HasOne<Conteudo>()
            .WithMany()
            .HasForeignKey(cc => cc.ColetaneaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Item FK: restrict delete (items NOT deleted when collection deleted — per D-12, Pitfall 2)
        builder.HasOne<Conteudo>()
            .WithMany()
            .HasForeignKey(cc => cc.ConteudoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index for faster lookups by position within collection
        builder.HasIndex(cc => new { cc.ColetaneaId, cc.Posicao })
            .HasDatabaseName("idx_conteudo_coletanea_posicao");
    }
}
