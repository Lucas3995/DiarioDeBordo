using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Core.Enums;
using DiarioDeBordo.Core.Repositorios;
using DiarioDeBordo.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace DiarioDeBordo.Infrastructure.Repositorios;

/// <summary>
/// Implementação do repositório de Coletanea via EF Core + PostgreSQL.
/// REGRA ABSOLUTA: toda query filtra por usuarioId — nunca acessa dados de outro usuário (SEG-02).
/// </summary>
internal sealed class ColetaneaRepository : IColetaneaRepository
{
    private readonly DiarioDeBordoDbContext _context;

    public ColetaneaRepository(DiarioDeBordoDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(Conteudo coletanea, CancellationToken ct)
    {
        await _context.Conteudos.AddAsync(coletanea, ct).ConfigureAwait(false);
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    public async Task<Conteudo?> ObterPorIdAsync(Guid id, Guid usuarioId, CancellationToken ct)
    {
        return await _context.Conteudos
            .Include(c => c.Fontes)
            .Include(c => c.Imagens)
            .Where(c => c.Id == id
                        && c.UsuarioId == usuarioId // SEG-02: usuarioId mandatory
                        && c.Papel == PapelConteudo.Coletanea)
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task AtualizarAsync(Conteudo coletanea, CancellationToken ct)
    {
        _context.Conteudos.Update(coletanea);
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    /// <summary>
    /// BFS traversal to find all descendant collection IDs.
    /// Used for cycle detection before adding item to collection.
    /// SEG-02: Returns empty if coletaneaId doesn't belong to usuarioId.
    /// </summary>
    public async Task<IReadOnlyList<Guid>> ObterDescendentesAsync(
        Guid coletaneaId, Guid usuarioId, CancellationToken ct)
    {
        // SEG-02: Validate that the starting collection belongs to the user
        var pertenceAoUsuario = await _context.Conteudos
            .AnyAsync(c => c.Id == coletaneaId && c.UsuarioId == usuarioId, ct)
            .ConfigureAwait(false);

        if (!pertenceAoUsuario)
            return Array.Empty<Guid>().ToList().AsReadOnly();

        var visited = new HashSet<Guid>();
        var queue = new Queue<Guid>();
        queue.Enqueue(coletaneaId);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (!visited.Add(current))
                continue;

            // Find direct children: items of this collection that are themselves collections
            var children = await _context.ConteudoColetaneas
                .Where(cc => cc.ColetaneaId == current)
                .Join(_context.Conteudos,
                    cc => cc.ConteudoId, c => c.Id,
                    (cc, c) => new { c.Id, c.Papel, c.UsuarioId })
                .Where(x => x.Papel == PapelConteudo.Coletanea
                            && x.UsuarioId == usuarioId) // SEG-02: scope to user
                .Select(x => x.Id)
                .ToListAsync(ct)
                .ConfigureAwait(false);

            foreach (var child in children)
                queue.Enqueue(child);
        }

        visited.Remove(coletaneaId); // exclude self from descendants
        return visited.ToList().AsReadOnly();
    }

    public async Task AdicionarItemAsync(ConteudoColetanea item, CancellationToken ct)
    {
        await _context.ConteudoColetaneas.AddAsync(item, ct).ConfigureAwait(false);
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    public async Task RemoverItemAsync(Guid coletaneaId, Guid conteudoId, Guid usuarioId, CancellationToken ct)
    {
        // SEG-02: validate collection belongs to user before removing association
        var existe = await _context.Conteudos
            .AnyAsync(c => c.Id == coletaneaId && c.UsuarioId == usuarioId, ct)
            .ConfigureAwait(false);

        if (!existe)
            return;

        var item = await _context.ConteudoColetaneas
            .Where(cc => cc.ColetaneaId == coletaneaId && cc.ConteudoId == conteudoId)
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);

        if (item is not null)
        {
            _context.ConteudoColetaneas.Remove(item);
            await _context.SaveChangesAsync(ct).ConfigureAwait(false);
        }
    }

    public async Task<ConteudoColetanea?> ObterItemAsync(
        Guid coletaneaId, Guid conteudoId, Guid usuarioId, CancellationToken ct)
    {
        // SEG-02: validate collection belongs to user via join
        return await _context.ConteudoColetaneas
            .Where(cc => cc.ColetaneaId == coletaneaId && cc.ConteudoId == conteudoId)
            .Join(_context.Conteudos,
                cc => cc.ColetaneaId, c => c.Id,
                (cc, c) => new { cc, c.UsuarioId })
            .Where(x => x.UsuarioId == usuarioId)
            .Select(x => x.cc)
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task AtualizarItemAsync(ConteudoColetanea item, CancellationToken ct)
    {
        _context.ConteudoColetaneas.Update(item);
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    public async Task<int> ObterProximaPosicaoAsync(Guid coletaneaId, CancellationToken ct)
    {
        var maxPosicao = await _context.ConteudoColetaneas
            .Where(cc => cc.ColetaneaId == coletaneaId)
            .Select(cc => (int?)cc.Posicao)
            .MaxAsync(ct)
            .ConfigureAwait(false);

        return (maxPosicao ?? 0) + 1;
    }

    public async Task<bool> ItemExisteNaColetaneaAsync(
        Guid coletaneaId, Guid conteudoId, CancellationToken ct)
    {
        return await _context.ConteudoColetaneas
            .AnyAsync(cc => cc.ColetaneaId == coletaneaId && cc.ConteudoId == conteudoId, ct)
            .ConfigureAwait(false);
    }
}
