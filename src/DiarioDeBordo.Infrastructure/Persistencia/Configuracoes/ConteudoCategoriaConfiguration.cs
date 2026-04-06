using DiarioDeBordo.Core.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiarioDeBordo.Infrastructure.Persistencia.Configuracoes;

internal sealed class ConteudoCategoriaConfiguration : IEntityTypeConfiguration<ConteudoCategoria>
{
    public void Configure(EntityTypeBuilder<ConteudoCategoria> builder)
    {
        builder.ToTable("conteudo_categorias");

        // Composite PK — no surrogate key needed for join table
        builder.HasKey(cc => new { cc.ConteudoId, cc.CategoriaId });

        builder.Property(cc => cc.ConteudoId)
            .HasColumnName("conteudo_id")
            .IsRequired();

        builder.Property(cc => cc.CategoriaId)
            .HasColumnName("categoria_id")
            .IsRequired();

        builder.Property(cc => cc.AssociadaEm)
            .HasColumnName("associada_em")
            .IsRequired();

        // FKs: Restrict — category associations must be removed before deleting content/category
        builder.HasOne<Conteudo>()
            .WithMany()
            .HasForeignKey(cc => cc.ConteudoId)
            .OnDelete(DeleteBehavior.Cascade); // Deleting content cascades category associations

        builder.HasOne<Categoria>()
            .WithMany()
            .HasForeignKey(cc => cc.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict); // Deleting category must be intentional
    }
}
