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
}
