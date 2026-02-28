using DiarioDeBordo.Domain.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiarioDeBordo.Persistence.Configurations;

/// <summary>
/// Configuração EF Core para a entidade Usuario.
///
/// LGPD: a tabela "Usuarios" contém dados pessoais (Login identifica o titular).
/// SenhaHash armazena exclusivamente o hash BCrypt — nunca a senha em claro.
/// </summary>
public sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Login)
               .IsRequired()
               .HasMaxLength(100);

        builder.HasIndex(u => u.Login)
               .IsUnique();

        builder.Property(u => u.SenhaHash)
               .IsRequired()
               .HasMaxLength(256);

        builder.Property(u => u.Perfil)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.Property(u => u.Ativo)
               .IsRequired()
               .HasDefaultValue(true);

        builder.Property(u => u.Requer2FA)
               .IsRequired()
               .HasDefaultValue(false);
    }
}
