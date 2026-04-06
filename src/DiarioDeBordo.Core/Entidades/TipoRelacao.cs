using DiarioDeBordo.Core.Primitivos;

namespace DiarioDeBordo.Core.Entidades;

/// <summary>
/// Tipo de relação entre conteúdos (ex: "Sequência" ↔ "Continuação de").
/// Deduplicação case-insensitive via NomeNormalizado.
/// Sistema: IsSistema=true para tipos pré-definidos (UsuarioId pode ser Guid.Empty).
/// </summary>
public sealed class TipoRelacao
{
    public Guid Id { get; init; }
    public Guid UsuarioId { get; init; }

    /// <summary>Nome da relação no sentido direto (ex: "Sequência").</summary>
    public required string Nome { get; init; }

    /// <summary>Nome da relação no sentido inverso (ex: "Continuação de").</summary>
    public required string NomeInverso { get; init; }

    /// <summary>Nome normalizado para deduplicação case-insensitive (lowercase, trimmed).</summary>
    public required string NomeNormalizado { get; init; }

    /// <summary>Tipo pré-definido pelo sistema. Disponível para todos os usuários.</summary>
    public bool IsSistema { get; init; }

    public static TipoRelacao Criar(Guid usuarioId, string nome, string nomeInverso, bool isSistema = false)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("NOME_TIPO_RELACAO_OBRIGATORIO", "Nome do tipo de relação é obrigatório.");

        if (string.IsNullOrWhiteSpace(nomeInverso))
            throw new DomainException("NOME_INVERSO_OBRIGATORIO", "Nome inverso do tipo de relação é obrigatório.");

        return new TipoRelacao
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuarioId,
            Nome = nome.Trim(),
            NomeInverso = nomeInverso.Trim(),
            NomeNormalizado = nome.Trim().ToLowerInvariant(),
            IsSistema = isSistema,
        };
    }
}
