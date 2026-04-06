using DiarioDeBordo.Core.Entidades;

namespace DiarioDeBordo.Core.Repositorios;

/// <summary>
/// Repositório de tipos de relação entre conteúdos.
/// Inclui tipos de sistema (IsSistema=true) disponíveis para todos os usuários.
/// SEG-02: all queries include usuarioId filter (with OR IsSistema=true for system types).
/// </summary>
public interface ITipoRelacaoRepository
{
    /// <summary>
    /// Operação atômica: retorna o TipoRelacao existente com o mesmo nome (case-insensitive)
    /// para o usuário (ou tipo de sistema) ou cria um novo.
    /// </summary>
    Task<TipoRelacao> ObterOuCriarAsync(Guid usuarioId, string nome, string nomeInverso, CancellationToken ct);

    /// <summary>
    /// Lista tipos de relação com autocomplete: tipos do usuário + tipos de sistema que correspondem ao prefixo.
    /// </summary>
    Task<IReadOnlyList<TipoRelacao>> ListarComAutocompletarAsync(Guid usuarioId, string prefixo, CancellationToken ct);

    Task<TipoRelacao?> ObterPorIdAsync(Guid id, Guid usuarioId, CancellationToken ct);
}
