using DiarioDeBordo.Domain.Obras;

namespace DiarioDeBordo.Application.Obras.Listar;

/// <summary>
/// Interface de repositório de leitura para obras — pertence à camada de Application
/// seguindo a direção de dependência da Clean Architecture:
/// Application define o contrato; Persistence implementa.
/// </summary>
public interface IObraLeituraRepository
{
    /// <summary>
    /// Retorna uma página de obras ordenadas por <c>OrdemPreferencia</c> (crescente).
    /// </summary>
    Task<(IReadOnlyList<Obra> Itens, int TotalCount)> ListarPaginadoAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);
}
