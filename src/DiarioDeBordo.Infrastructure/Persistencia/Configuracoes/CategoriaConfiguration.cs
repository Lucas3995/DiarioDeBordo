using DiarioDeBordo.Core.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiarioDeBordo.Infrastructure.Persistencia.Configuracoes;

internal sealed class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("categorias");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(c => c.UsuarioId)
            .HasColumnName("usuario_id")
            .IsRequired();

        builder.Property(c => c.Nome)
            .HasColumnName("nome")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.NomeNormalizado)
            .HasColumnName("nome_normalizado")
            .HasMaxLength(200)
            .IsRequired();

        // I-12: unique per user, case-insensitive (NomeNormalizado is always lowercase)
        builder.HasIndex(c => new { c.UsuarioId, c.NomeNormalizado })
            .IsUnique()
            .HasDatabaseName("idx_categorias_usuario_nome_unique");
    }
}
