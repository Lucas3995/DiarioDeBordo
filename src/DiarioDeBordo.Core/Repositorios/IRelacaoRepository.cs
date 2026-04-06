using DiarioDeBordo.Core.Entidades;

namespace DiarioDeBordo.Core.Repositorios;

/// <summary>
/// Repositório de relações bidirecionais entre conteúdos.
/// SEG-02: all queries include usuarioId filter.
/// Two-row pattern: forward + inverse stored as separate Relacao rows linked by ParId.
/// </summary>
public interface IRelacaoRepository
{
    /// <summary>
    /// Adiciona o par bidirecional (forward + inverse) atomicamente em uma única operação.
    /// Não chama SaveChangesAsync — responsabilidade do handler (Unit of Work).
    /// </summary>
    Task AdicionarParAsync(Relacao forward, Relacao inverse, CancellationToken ct);

    /// <summary>
    /// Lista todas as relações onde o conteúdo é origem (IsInversa=false) para evitar duplicatas na exibição.
    /// </summary>
    Task<IReadOnlyList<Relacao>> ListarPorConteudoAsync(Guid conteudoId, Guid usuarioId, CancellationToken ct);

    /// <summary>
    /// Verifica se já existe uma relação entre dois conteúdos com o mesmo tipo.
    /// </summary>
    Task<bool> ExisteAsync(Guid conteudoOrigemId, Guid conteudoDestinoId, Guid tipoRelacaoId, Guid usuarioId, CancellationToken ct);

    /// <summary>
    /// Remove o par bidirecional (WHERE Id = relacaoId OR ParId = relacaoId).
    /// </summary>
    Task RemoverParAsync(Guid relacaoId, Guid usuarioId, CancellationToken ct);
}
