using DiarioDeBordo.Core.Consultas;
using DiarioDeBordo.Core.Enums;
using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace DiarioDeBordo.Infrastructure.Consultas;

/// <summary>
/// Implementação somente-leitura de consultas de Conteudo (CQRS — query side).
/// REGRA: toda query filtra por usuarioId — nunca acessa dados de outro usuário. (SEG-02)
/// </summary>
internal sealed class ConteudoQueryService : IConteudoQueryService
{
    private readonly DiarioDeBordoDbContext _context;

    public ConteudoQueryService(DiarioDeBordoDbContext context)
    {
        _context = context;
    }

    public async Task<ResultadoPaginado<ConteudoResumoData>> ListarAsync(
        Guid usuarioId, PaginacaoParams paginacao, CancellationToken ct)
    {
        var total = await _context.Conteudos
            .CountAsync(c => c.UsuarioId == usuarioId, ct)
            .ConfigureAwait(false);

        var items = await _context.Conteudos
            .Where(c => c.UsuarioId == usuarioId) // SEG-02: usuarioId mandatory
            .OrderByDescending(c => c.CriadoEm)
            .Skip(paginacao.Offset)
            .Take(paginacao.ItensPorPagina)
            .Select(c => new ConteudoResumoData(c.Id, c.Titulo, c.Formato, c.Papel, c.CriadoEm))
            .ToListAsync(ct)
            .ConfigureAwait(false);

        return new ResultadoPaginado<ConteudoResumoData>(
            items.AsReadOnly(), total, paginacao.Pagina, paginacao.ItensPorPagina);
    }

    public async Task<ConteudoDetalheData?> ObterAsync(Guid id, Guid usuarioId, CancellationToken ct)
    {
        return await _context.Conteudos
            .Where(c => c.Id == id && c.UsuarioId == usuarioId) // SEG-02: usuarioId mandatory
            .Select(c => new ConteudoDetalheData(
                c.Id,
                c.Titulo,
                c.Descricao,
                c.Anotacoes,
                c.Nota,
                c.Formato,
                c.Papel,
                c.CriadoEm))
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);
    }
}
