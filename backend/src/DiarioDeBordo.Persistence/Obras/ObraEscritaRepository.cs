using DiarioDeBordo.Domain.Obras;
using Microsoft.EntityFrameworkCore;

namespace DiarioDeBordo.Persistence.Obras;

/// <summary>
/// Implementação EF Core de IObraEscritaRepository.
/// Usa tracking para permitir atualização da mesma instância obtida por id/nome.
/// </summary>
public sealed class ObraEscritaRepository(DiarioDeBordoDbContext dbContext) : IObraEscritaRepository
{
    public async Task<Obra?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Obras
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<Obra?> ObterPorNomeAsync(string nome, CancellationToken cancellationToken = default)
    {
        return await dbContext.Obras
            .FirstOrDefaultAsync(o => o.Nome == nome, cancellationToken);
    }

    public async Task AdicionarAsync(Obra obra, CancellationToken cancellationToken = default)
    {
        await dbContext.Obras.AddAsync(obra, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(Obra obra, CancellationToken cancellationToken = default)
    {
        dbContext.Obras.Update(obra);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
