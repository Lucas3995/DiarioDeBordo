namespace DiarioDeBordo.Domain.Obras;

/// <summary>
/// Repositório de escrita para obras: obter por id/nome e persistir (adicionar/atualizar).
/// Domain define o contrato; Persistence implementa.
/// Compartilhado pelos casos de uso AtualizarPosicao e ObterPorIdOuNome.
/// </summary>
public interface IObraEscritaRepository
{
    Task<Obra?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Obra?> ObterPorNomeAsync(string nome, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Obra obra, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Obra obra, CancellationToken cancellationToken = default);
}
