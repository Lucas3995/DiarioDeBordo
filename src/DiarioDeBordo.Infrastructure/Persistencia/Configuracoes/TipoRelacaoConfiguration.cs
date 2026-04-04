using DiarioDeBordo.Core.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiarioDeBordo.Infrastructure.Persistencia.Configuracoes;

internal sealed class TipoRelacaoConfiguration : IEntityTypeConfiguration<TipoRelacao>
{
    // Well-known deterministic Guids for system-seeded relation types (D-13)
    private static readonly Guid _sequencia = Guid.Parse("10000000-0000-0000-0000-000000000001");
    private static readonly Guid _derivadoDe = Guid.Parse("10000000-0000-0000-0000-000000000002");
    private static readonly Guid _referenciadoEm = Guid.Parse("10000000-0000-0000-0000-000000000003");
    private static readonly Guid _adaptacaoDe = Guid.Parse("10000000-0000-0000-0000-000000000004");
    private static readonly Guid _alternativaA = Guid.Parse("10000000-0000-0000-0000-000000000005");
    private static readonly Guid _doMesmoTipoQue = Guid.Parse("10000000-0000-0000-0000-000000000006");
    private static readonly Guid _complementoDe = Guid.Parse("10000000-0000-0000-0000-000000000007");
    private static readonly Guid _preRequisitoParа = Guid.Parse("10000000-0000-0000-0000-000000000008");
    private static readonly Guid _contem = Guid.Parse("10000000-0000-0000-0000-000000000009");

    public void Configure(EntityTypeBuilder<TipoRelacao> builder)
    {
        builder.ToTable("tipo_relacoes");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(t => t.UsuarioId)
            .HasColumnName("usuario_id")
            .IsRequired();

        builder.Property(t => t.Nome)
            .HasColumnName("nome")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.NomeInverso)
            .HasColumnName("nome_inverso")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.NomeNormalizado)
            .HasColumnName("nome_normalizado")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.IsSistema)
            .HasColumnName("is_sistema")
            .IsRequired()
            .HasDefaultValue(false);

        // Unique per user, case-insensitive (NomeNormalizado is always lowercase)
        builder.HasIndex(t => new { t.UsuarioId, t.NomeNormalizado })
            .IsUnique()
            .HasDatabaseName("idx_tipo_relacoes_usuario_nome_unique");

        // Seed data: 9 predefined system relation types (D-13 + D-18 "Contém"/"Parte de")
        builder.HasData(
            Seed(_sequencia, "Sequência", "Continuação de"),
            Seed(_derivadoDe, "Derivado de", "Derivou"),
            Seed(_referenciadoEm, "Referenciado em", "Referencia"),
            Seed(_adaptacaoDe, "Adaptação de", "Adaptado em"),
            Seed(_alternativaA, "Alternativa a", "Alternativa a"),         // symmetric
            Seed(_doMesmoTipoQue, "Do mesmo tipo que", "Do mesmo tipo que"), // symmetric
            Seed(_complementoDe, "Complemento de", "Complementado por"),
            Seed(_preRequisitoParа, "Pré-requisito para", "Requer"),
            Seed(_contem, "Contém", "Parte de")                            // D-18: sessions
        );
    }

    private static TipoRelacao Seed(Guid id, string nome, string nomeInverso) => new()
    {
        Id = id,
        UsuarioId = Guid.Empty,
        Nome = nome,
        NomeInverso = nomeInverso,
        NomeNormalizado = nome.Trim().ToLowerInvariant(),
        IsSistema = true,
    };
}
