using DiarioDeBordo.Domain.Common;

namespace DiarioDeBordo.Domain.Interfaces;

/// <summary>
/// Contrato base de repositório orientado ao domínio (DDD).
/// Esconde detalhes de persistência das camadas internas.
/// </summary>
public interface IRepository<TEntity> where TEntity : Entity
{
    Task<TEntity?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> ListarTodosAsync(CancellationToken cancellationToken = default);
    Task AdicionarAsync(TEntity entidade, CancellationToken cancellationToken = default);
    Task AtualizarAsync(TEntity entidade, CancellationToken cancellationToken = default);
    Task RemoverAsync(TEntity entidade, CancellationToken cancellationToken = default);
}
