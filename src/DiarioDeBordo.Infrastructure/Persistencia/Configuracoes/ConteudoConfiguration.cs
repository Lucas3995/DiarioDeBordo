using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiarioDeBordo.Infrastructure.Persistencia.Configuracoes;

internal sealed class ConteudoConfiguration : IEntityTypeConfiguration<Conteudo>
{
    public void Configure(EntityTypeBuilder<Conteudo> builder)
    {
        builder.ToTable("conteudos");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(c => c.UsuarioId)
            .HasColumnName("usuario_id")
            .IsRequired();

        builder.Property(c => c.Titulo)
            .HasColumnName("titulo")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(c => c.Descricao)
            .HasColumnName("descricao")
            .HasMaxLength(5000);

        builder.Property(c => c.Anotacoes)
            .HasColumnName("anotacoes")
            .HasMaxLength(10000);

        builder.Property(c => c.Nota)
            .HasColumnName("nota")
            .HasPrecision(4, 1);

        builder.Property(c => c.Formato)
            .HasColumnName("formato")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.Subtipo)
            .HasColumnName("subtipo")
            .HasMaxLength(100);

        builder.Property(c => c.Papel)
            .HasColumnName("papel")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.TipoColetaneaValor)
            .HasColumnName("tipo_coletanea")
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(c => c.CriadoEm)
            .HasColumnName("criado_em")
            .IsRequired();

        builder.Property(c => c.AtualizadoEm)
            .HasColumnName("atualizado_em")
            .IsRequired();

        // Progresso — owned entity (value object), stored inline in conteudos table
        builder.OwnsOne(c => c.Progresso, p =>
        {
            p.Property(x => x.Estado)
                .HasColumnName("progresso_estado")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            p.Property(x => x.PosicaoAtual)
                .HasColumnName("progresso_posicao_atual")
                .HasMaxLength(500);

            p.Property(x => x.NotaManual)
                .HasColumnName("progresso_nota_manual")
                .HasMaxLength(5000);
        });

        // Indexes — SEG-02: every query filters by usuarioId, so index is critical
        builder.HasIndex(c => c.UsuarioId)
            .HasDatabaseName("idx_conteudos_usuario_id");

        builder.HasIndex(c => new { c.UsuarioId, c.CriadoEm })
            .HasDatabaseName("idx_conteudos_usuario_criado_em");

        // D-19: filter IsFilho from list — composite index for performance
        builder.HasIndex(c => new { c.UsuarioId, c.IsFilho })
            .HasDatabaseName("idx_conteudos_usuario_is_filho");

        // New columns: Classificacao, IsFilho, TotalEsperadoSessoes
        builder.Property(c => c.Classificacao)
            .HasColumnName("classificacao")
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(c => c.IsFilho)
            .HasColumnName("is_filho")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.TotalEsperadoSessoes)
            .HasColumnName("total_esperado_sessoes");

        // Fontes — owned collection stored in separate table
        builder.Navigation(c => c.Fontes)
            .HasField("_fontes")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(c => c.Fontes, f =>
        {
            f.ToTable("fontes");
            f.HasKey(x => x.Id);

            f.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            f.WithOwner()
                .HasForeignKey(x => x.ConteudoId);

            f.Property(x => x.ConteudoId)
                .HasColumnName("conteudo_id")
                .IsRequired();

            f.Property(x => x.Tipo)
                .HasColumnName("tipo")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            f.Property(x => x.Valor)
                .HasColumnName("valor")
                .HasMaxLength(2000)
                .IsRequired();

            f.Property(x => x.Plataforma)
                .HasColumnName("plataforma")
                .HasMaxLength(100);

            f.Property(x => x.Prioridade)
                .HasColumnName("prioridade")
                .IsRequired();

            // I-07: unique priority per content
            f.HasIndex(x => new { x.ConteudoId, x.Prioridade })
                .IsUnique()
                .HasDatabaseName("idx_fontes_conteudo_prioridade_unique");
        });

        // Imagens — owned collection stored in separate table
        builder.Navigation(c => c.Imagens)
            .HasField("_imagens")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(c => c.Imagens, img =>
        {
            img.ToTable("imagens_conteudo");
            img.HasKey(x => x.Id);

            img.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            img.WithOwner()
                .HasForeignKey(x => x.ConteudoId);

            img.Property(x => x.ConteudoId)
                .HasColumnName("conteudo_id")
                .IsRequired();

            img.Property(x => x.OrigemTipo)
                .HasColumnName("origem_tipo")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            img.Property(x => x.Caminho)
                .HasColumnName("caminho")
                .HasMaxLength(1000)
                .IsRequired();

            img.Property(x => x.Principal)
                .HasColumnName("principal")
                .IsRequired();
        });
    }
}
