namespace DiarioDeBordo.Core.Infraestrutura;

/// <summary>
/// Gerencia o ciclo de vida do PostgreSQL bundled.
/// Sequência: initdb (first run) → start → migrate → healthcheck.
/// (Padrões Técnicos v4, seção 4.4 adaptado; D-01)
/// </summary>
public interface IPostgresBootstrap
{
    /// <summary>
    /// Garante que o PostgreSQL está rodando e com migrations aplicadas.
    /// Bloqueante — deve ser chamado antes da inicialização do container de DI.
    /// </summary>
    Task<bool> EnsureRunningAsync(CancellationToken ct);

    /// <summary>Verifica se o cluster já foi inicializado (pgdata existe e tem conteúdo).</summary>
    Task<bool> IsInitializedAsync();

    /// <summary>Verifica se o processo pg está aceitando conexões na porta 15432.</summary>
    Task<bool> IsRunningAsync();

    /// <summary>
    /// Constrói a connection string usando a senha armazenada em secure storage.
    /// Deve ser chamado após EnsureRunningAsync para garantir que a senha existe.
    /// </summary>
    Task<string> BuildConnectionStringAsync(CancellationToken ct);
}
