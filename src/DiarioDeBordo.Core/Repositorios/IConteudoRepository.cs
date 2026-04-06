using DiarioDeBordo.Core.Entidades;

namespace DiarioDeBordo.Core.Repositorios;

/// <summary>
/// Contrato de persistência para o agregado Conteudo.
/// REGRA: todo método inclui usuarioId — sem exceções (Padrões Técnicos v4, seção 4.6).
/// Implementado em DiarioDeBordo.Infrastructure.
/// </summary>
public interface IConteudoRepository
{
    Task AdicionarAsync(Conteudo conteudo, CancellationToken ct);
    Task<Conteudo?> ObterPorIdAsync(Guid id, Guid usuarioId, CancellationToken ct);
    Task AtualizarAsync(Conteudo conteudo, CancellationToken ct);
    Task RemoverAsync(Guid id, Guid usuarioId, CancellationToken ct);
    Task<Conteudo?> BuscarPorUrlFonteAsync(Guid usuarioId, string urlNormalizada, CancellationToken ct);
    Task<Conteudo?> BuscarPorIdentificadorFonteAsync(Guid usuarioId, string plataforma, string identificador, CancellationToken ct);

    /// <summary>
    /// Substitui as categorias associadas ao conteúdo pelo conjunto desejado.
    /// Add new associations and removes deassociated ones.
    /// SEG-02: usuarioId included for authorization.
    /// </summary>
    Task AtualizarCategoriasAsync(Guid conteudoId, Guid usuarioId, IReadOnlyList<Guid> categoriaIds, CancellationToken ct);

    /// <summary>
    /// Remove todas as categorias associadas ao conteúdo (usado no delete cascade).
    /// </summary>
    Task RemoverTodasCategoriasAsync(Guid conteudoId, CancellationToken ct);

    /// <summary>
    /// Lista conteúdos filhos (IsFilho=true) vinculados a este conteúdo via relação "Contém".
    /// Usado para cascade delete.
    /// </summary>
    Task<IReadOnlyList<Guid>> ListarFilhosAsync(Guid conteudoId, Guid usuarioId, Guid contemTipoRelacaoId, CancellationToken ct);
}
