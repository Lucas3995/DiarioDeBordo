using DiarioDeBordo.Core.Entidades;

namespace DiarioDeBordo.Core.Repositorios;

public interface ICategoriaRepository
{
    /// <summary>
    /// Operação atômica: retorna a Categoria existente com o mesmo nome (case-insensitive)
    /// ou cria uma nova. Implementa I-12.
    /// </summary>
    Task<Categoria> ObterOuCriarAsync(Guid usuarioId, string nome, CancellationToken ct);

    Task<IReadOnlyList<Categoria>> ListarComAutocompletarAsync(Guid usuarioId, string prefixo, CancellationToken ct);
}
