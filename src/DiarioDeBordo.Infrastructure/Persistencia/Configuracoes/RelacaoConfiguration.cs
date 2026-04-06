using DiarioDeBordo.Core.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiarioDeBordo.Infrastructure.Persistencia.Configuracoes;

internal sealed class RelacaoConfiguration : IEntityTypeConfiguration<Relacao>
{
    public void Configure(EntityTypeBuilder<Relacao> builder)
    {
        builder.ToTable("relacoes");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(r => r.ConteudoOrigemId)
            .HasColumnName("conteudo_origem_id")
            .IsRequired();

        builder.Property(r => r.ConteudoDestinoId)
            .HasColumnName("conteudo_destino_id")
            .IsRequired();

        builder.Property(r => r.TipoRelacaoId)
            .HasColumnName("tipo_relacao_id")
            .IsRequired();

        builder.Property(r => r.IsInversa)
            .HasColumnName("is_inversa")
            .IsRequired();

        builder.Property(r => r.ParId)
            .HasColumnName("par_id");

        builder.Property(r => r.UsuarioId)
            .HasColumnName("usuario_id")
            .IsRequired();

        builder.Property(r => r.CriadoEm)
            .HasColumnName("criado_em")
            .IsRequired();

        // FK: origem → conteudos (Restrict — relations must be removed before deleting content)
        builder.HasOne<Conteudo>()
            .WithMany()
            .HasForeignKey(r => r.ConteudoOrigemId)
            .OnDelete(DeleteBehavior.Restrict);

        // FK: destino → conteudos (Restrict)
        builder.HasOne<Conteudo>()
            .WithMany()
            .HasForeignKey(r => r.ConteudoDestinoId)
            .OnDelete(DeleteBehavior.Restrict);

        // FK: tipo → tipo_relacoes (Restrict)
        builder.HasOne<TipoRelacao>()
            .WithMany()
            .HasForeignKey(r => r.TipoRelacaoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unique: prevent duplicate relations (same origin, destination, type)
        builder.HasIndex(r => new { r.ConteudoOrigemId, r.ConteudoDestinoId, r.TipoRelacaoId })
            .IsUnique()
            .HasDatabaseName("idx_relacoes_origem_destino_tipo_unique");

        // Performance: listing relations for a content
        builder.HasIndex(r => new { r.ConteudoOrigemId, r.UsuarioId })
            .HasDatabaseName("idx_relacoes_origem_usuario");

        // Performance: pair lookup for bidirectional delete
        builder.HasIndex(r => r.ParId)
            .HasDatabaseName("idx_relacoes_par_id");
    }
}
