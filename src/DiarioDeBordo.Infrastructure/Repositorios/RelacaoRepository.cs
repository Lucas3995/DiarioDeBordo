using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Core.Repositorios;
using DiarioDeBordo.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace DiarioDeBordo.Infrastructure.Repositorios;

internal sealed class RelacaoRepository : IRelacaoRepository
{
    private readonly DiarioDeBordoDbContext _context;

    public RelacaoRepository(DiarioDeBordoDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Adiciona forward + inverse atomicamente em uma única transação (Pitfall 3 — evitar dois SaveChanges).
    /// </summary>
    public async Task AdicionarParAsync(Relacao forward, Relacao inverse, CancellationToken ct)
    {
        await _context.Relacoes.AddAsync(forward, ct).ConfigureAwait(false);
        await _context.Relacoes.AddAsync(inverse, ct).ConfigureAwait(false);
        await _context.SaveChangesAsync(ct).ConfigureAwait(false); // Single SaveChanges = single transaction
    }

    /// <summary>
    /// Lista relações onde o conteúdo é origem (IsInversa=false) para evitar duplicatas na exibição.
    /// SEG-02: usuarioId mandatory.
    /// </summary>
    public async Task<IReadOnlyList<Relacao>> ListarPorConteudoAsync(
        Guid conteudoId, Guid usuarioId, CancellationToken ct)
    {
        return await _context.Relacoes
            .Where(r => r.ConteudoOrigemId == conteudoId && r.UsuarioId == usuarioId)
            .OrderByDescending(r => r.CriadoEm)
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }

    /// <summary>SEG-02: includes usuarioId in query.</summary>
    public async Task<bool> ExisteAsync(
        Guid conteudoOrigemId, Guid conteudoDestinoId, Guid tipoRelacaoId, Guid usuarioId, CancellationToken ct)
    {
        return await _context.Relacoes
            .AnyAsync(r =>
                r.ConteudoOrigemId == conteudoOrigemId &&
                r.ConteudoDestinoId == conteudoDestinoId &&
                r.TipoRelacaoId == tipoRelacaoId &&
                r.UsuarioId == usuarioId,
                ct)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Remove ambos os lados do par bidirecional: WHERE Id == relacaoId OR ParId == relacaoId.
    /// SEG-02: usuarioId filter prevents cross-user deletion.
    /// </summary>
    public async Task RemoverParAsync(Guid relacaoId, Guid usuarioId, CancellationToken ct)
    {
        var relacoes = await _context.Relacoes
            .Where(r => (r.Id == relacaoId || r.ParId == relacaoId) && r.UsuarioId == usuarioId)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        _context.Relacoes.RemoveRange(relacoes);
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}
