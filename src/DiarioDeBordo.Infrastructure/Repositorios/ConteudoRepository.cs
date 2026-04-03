using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Core.Repositorios;
using DiarioDeBordo.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace DiarioDeBordo.Infrastructure.Repositorios;

/// <summary>
/// Implementação do repositório de Conteudo via EF Core + PostgreSQL.
/// REGRA ABSOLUTA: toda query filtra por usuarioId — nunca acessa dados de outro usuário.
/// (Padrões Técnicos v4, seção 4.6)
/// </summary>
internal sealed class ConteudoRepository : IConteudoRepository
{
    private readonly DiarioDeBordoDbContext _context;

    public ConteudoRepository(DiarioDeBordoDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(Conteudo conteudo, CancellationToken ct)
    {
        await _context.Conteudos.AddAsync(conteudo, ct).ConfigureAwait(false);
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    public async Task<Conteudo?> ObterPorIdAsync(Guid id, Guid usuarioId, CancellationToken ct)
    {
        return await _context.Conteudos
            .Where(c => c.Id == id && c.UsuarioId == usuarioId) // usuarioId MANDATORY — SEG-02
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task AtualizarAsync(Conteudo conteudo, CancellationToken ct)
    {
        _context.Conteudos.Update(conteudo);
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    public async Task RemoverAsync(Guid id, Guid usuarioId, CancellationToken ct)
    {
        var conteudo = await ObterPorIdAsync(id, usuarioId, ct).ConfigureAwait(false);
        if (conteudo is not null)
        {
            _context.Conteudos.Remove(conteudo);
            await _context.SaveChangesAsync(ct).ConfigureAwait(false);
        }
    }

    public async Task<Conteudo?> BuscarPorUrlFonteAsync(Guid usuarioId, string urlNormalizada, CancellationToken ct)
    {
        return await _context.Conteudos
            .Where(c => c.UsuarioId == usuarioId) // usuarioId MANDATORY — SEG-02
            .Where(c => c.Fontes.Any(f => f.Valor == urlNormalizada))
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task<Conteudo?> BuscarPorIdentificadorFonteAsync(
        Guid usuarioId, string plataforma, string identificador, CancellationToken ct)
    {
        return await _context.Conteudos
            .Where(c => c.UsuarioId == usuarioId) // usuarioId MANDATORY — SEG-02
            .Where(c => c.Fontes.Any(f => f.Plataforma == plataforma && f.Valor == identificador))
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);
    }
}
