using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Core.Repositorios;
using DiarioDeBordo.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace DiarioDeBordo.Infrastructure.Repositorios;

internal sealed class CategoriaRepository : ICategoriaRepository
{
    private readonly DiarioDeBordoDbContext _context;

    public CategoriaRepository(DiarioDeBordoDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// I-12: Operação atômica — retorna existente ou cria novo.
    /// NomeNormalizado é sempre lowercase para garantir unicidade case-insensitive.
    /// </summary>
    public async Task<Categoria> ObterOuCriarAsync(Guid usuarioId, string nome, CancellationToken ct)
    {
        var nomeNormalizado = nome.Trim().ToLowerInvariant();

        var existente = await _context.Categorias
            .Where(c => c.UsuarioId == usuarioId && c.NomeNormalizado == nomeNormalizado)
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);

        if (existente is not null)
            return existente;

        var nova = Categoria.Criar(usuarioId, nome);
        await _context.Categorias.AddAsync(nova, ct).ConfigureAwait(false);
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);
        return nova;
    }

    public async Task<IReadOnlyList<Categoria>> ListarComAutocompletarAsync(
        Guid usuarioId, string prefixo, CancellationToken ct)
    {
        var prefixoNormalizado = prefixo.Trim().ToLowerInvariant();

        return await _context.Categorias
            .Where(c => c.UsuarioId == usuarioId
                        && c.NomeNormalizado.StartsWith(prefixoNormalizado))
            .OrderBy(c => c.Nome)
            .Take(20)
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }
}
