using DiarioDeBordo.Core.Entidades;

namespace DiarioDeBordo.Core.Repositorios;

public interface IColetaneaRepository
{
    Task AdicionarAsync(Conteudo coletanea, CancellationToken ct);
    Task<Conteudo?> ObterPorIdAsync(Guid id, Guid usuarioId, CancellationToken ct);
    Task<IReadOnlyList<Guid>> ObterDescendentesAsync(Guid coletaneaId, Guid usuarioId, CancellationToken ct);
    Task AtualizarAsync(Conteudo coletanea, CancellationToken ct);
}
