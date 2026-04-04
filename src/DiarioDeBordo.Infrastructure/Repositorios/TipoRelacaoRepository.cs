using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Core.Repositorios;
using DiarioDeBordo.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace DiarioDeBordo.Infrastructure.Repositorios;

internal sealed class TipoRelacaoRepository : ITipoRelacaoRepository
{
    private readonly DiarioDeBordoDbContext _context;

    public TipoRelacaoRepository(DiarioDeBordoDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Operação atômica: retorna tipo existente (do usuário ou sistema) com mesmo nome (case-insensitive),
    /// ou cria um novo tipo para o usuário.
    /// System types matched by name are returned directly (no user copy created).
    /// </summary>
    public async Task<TipoRelacao> ObterOuCriarAsync(
        Guid usuarioId, string nome, string nomeInverso, CancellationToken ct)
    {
        var nomeNormalizado = nome.Trim().ToLowerInvariant();

        var existente = await _context.TipoRelacoes
            .Where(t => (t.UsuarioId == usuarioId || t.IsSistema) && t.NomeNormalizado == nomeNormalizado)
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);

        if (existente is not null)
            return existente;

        var novo = TipoRelacao.Criar(usuarioId, nome, nomeInverso);
        await _context.TipoRelacoes.AddAsync(novo, ct).ConfigureAwait(false);
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);
        return novo;
    }

    /// <summary>
    /// Lista tipos de relação com autocomplete: tipos do usuário + tipos de sistema que correspondem ao prefixo.
    /// </summary>
    public async Task<IReadOnlyList<TipoRelacao>> ListarComAutocompletarAsync(
        Guid usuarioId, string prefixo, CancellationToken ct)
    {
        var prefixoNormalizado = prefixo.Trim().ToLowerInvariant();

        return await _context.TipoRelacoes
            .Where(t => (t.UsuarioId == usuarioId || t.IsSistema)
                        && t.NomeNormalizado.StartsWith(prefixoNormalizado))
            .OrderBy(t => t.Nome)
            .Take(20)
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task<TipoRelacao?> ObterPorIdAsync(Guid id, Guid usuarioId, CancellationToken ct)
    {
        return await _context.TipoRelacoes
            .Where(t => t.Id == id && (t.UsuarioId == usuarioId || t.IsSistema))
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);
    }
}
