using DiarioDeBordo.Application.Obras.Listar;
using DiarioDeBordo.Domain.Obras;
using Microsoft.EntityFrameworkCore;

namespace DiarioDeBordo.Persistence.Obras;

/// <summary>
/// Implementação EF Core de IObraLeituraRepository.
/// Reside em Persistence, garantindo que Application nunca dependa de EF/DbContext.
/// Queries são AsNoTracking (somente leitura, performance).
/// </summary>
public sealed class ObraLeituraRepository(DiarioDeBordoDbContext dbContext)
    : IObraLeituraRepository
{
    public async Task<(IReadOnlyList<Obra> Itens, int TotalCount)> ListarPaginadoAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Obras
            .AsNoTracking()
            .OrderBy(o => o.OrdemPreferencia);

        var totalCount = await query.CountAsync(cancellationToken);

        var itens = await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (itens, totalCount);
    }

    public async Task<IReadOnlyList<Obra>> BuscarPorNomeAsync(
        string? termo,
        int limit,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Obra> baseQuery = dbContext.Obras.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(termo))
        {
            var filtro = termo.Trim().ToLowerInvariant();
            baseQuery = baseQuery.Where(o => o.Nome.ToLower().Contains(filtro));
        }

        var query = baseQuery.OrderBy(o => o.OrdemPreferencia);

        return await query
            .Take(limit)
            .ToListAsync(cancellationToken);
    }
}
