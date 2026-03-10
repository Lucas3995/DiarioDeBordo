using DiarioDeBordo.Domain.Obras;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiarioDeBordo.Persistence.Configurations;

/// <summary>
/// Mapeamento EF Core para a entidade Obra.
/// Enums armazenados como string para facilitar legibilidade no banco.
/// Datas em UTC (Npgsql mapeia DateTime UTC corretamente com Timestamptz).
/// </summary>
public sealed class ObraConfiguration : IEntityTypeConfiguration<Obra>
{
    public void Configure(EntityTypeBuilder<Obra> builder)
    {
        builder.ToTable("Obras");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .ValueGeneratedNever();

        builder.Property(o => o.Nome)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(o => o.Tipo)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(o => o.PosicaoAtual)
            .IsRequired();

        builder.Property(o => o.DataUltimaAtualizacaoPosicao)
            .IsRequired();

        builder.Property(o => o.OrdemPreferencia)
            .IsRequired();

        builder.Property(o => o.ProximaInfoTipo)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(o => o.DiasAteProximaParte);
        builder.Property(o => o.PartesJaPublicadas);

        builder.Property(o => o.CriadoEm)
            .IsRequired();

        builder.Property(o => o.AtualizadoEm)
            .IsRequired();
    }
}
