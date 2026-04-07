using DiarioDeBordo.Core.Entidades;

namespace DiarioDeBordo.Core.Repositorios;

public interface IColetaneaRepository
{
    Task AdicionarAsync(Conteudo coletanea, CancellationToken ct);
    Task<Conteudo?> ObterPorIdAsync(Guid id, Guid usuarioId, CancellationToken ct);
    Task<IReadOnlyList<Guid>> ObterDescendentesAsync(Guid coletaneaId, Guid usuarioId, CancellationToken ct);
    Task AtualizarAsync(Conteudo coletanea, CancellationToken ct);

    // ConteudoColetanea item management
    Task AdicionarItemAsync(ConteudoColetanea item, CancellationToken ct);
    Task RemoverItemAsync(Guid coletaneaId, Guid conteudoId, Guid usuarioId, CancellationToken ct);
    Task<ConteudoColetanea?> ObterItemAsync(Guid coletaneaId, Guid conteudoId, Guid usuarioId, CancellationToken ct);
    Task AtualizarItemAsync(ConteudoColetanea item, CancellationToken ct);
    Task<int> ObterProximaPosicaoAsync(Guid coletaneaId, CancellationToken ct);
    Task<bool> ItemExisteNaColetaneaAsync(Guid coletaneaId, Guid conteudoId, CancellationToken ct);
}
