namespace DiarioDeBordo.Domain.Common;

/// <summary>
/// Contrato de Unit of Work: garante consistência transacional entre repositórios.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
}
