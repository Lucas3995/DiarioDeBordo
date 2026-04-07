using DiarioDeBordo.Core.Consultas;
using DiarioDeBordo.Core.Enums;
using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace DiarioDeBordo.Infrastructure.Consultas;

/// <summary>
/// Read-side queries for collection data. CQRS separation from IColetaneaRepository.
/// REGRA: toda query filtra por usuarioId — nunca acessa dados de outro usuário (SEG-02).
/// </summary>
internal sealed class ColetaneaQueryService : IColetaneaQueryService
{
    private readonly DiarioDeBordoDbContext _context;

    public ColetaneaQueryService(DiarioDeBordoDbContext context)
    {
        _context = context;
    }

    public async Task<ResultadoPaginado<ColetaneaItemData>> ListarItensAsync(
        Guid coletaneaId, Guid usuarioId, PaginacaoParams paginacao, CancellationToken ct)
    {
        // Validate collection belongs to user first
        var coletaneaExiste = await _context.Conteudos
            .AnyAsync(c => c.Id == coletaneaId
                          && c.UsuarioId == usuarioId
                          && c.Papel == PapelConteudo.Coletanea, ct)
            .ConfigureAwait(false);

        if (!coletaneaExiste)
            return new ResultadoPaginado<ColetaneaItemData>(
                Array.Empty<ColetaneaItemData>().AsReadOnly(), 0, paginacao.Pagina, paginacao.ItensPorPagina);

        var query = _context.ConteudoColetaneas
            .Where(cc => cc.ColetaneaId == coletaneaId)
            .Join(_context.Conteudos,
                cc => cc.ConteudoId, c => c.Id,
                (cc, c) => new { cc, c });

        var total = await query.CountAsync(ct).ConfigureAwait(false);

        var items = await query
            .OrderBy(x => x.cc.Posicao)
            .Skip(paginacao.Offset)
            .Take(paginacao.ItensPorPagina)
            .Select(x => new ColetaneaItemData(
                x.c.Id,
                x.c.Titulo,
                x.cc.Posicao,
                x.cc.AnotacaoContextual,
                x.c.Progresso.Estado,
                x.c.Papel,
                x.c.TipoColetaneaValor,
                // SubItensContagem: count only if this item is itself a collection
                x.c.Papel == PapelConteudo.Coletanea
                    ? _context.ConteudoColetaneas.Count(sub => sub.ColetaneaId == x.c.Id)
                    : (int?)null,
                x.cc.AdicionadoEm))
            .ToListAsync(ct)
            .ConfigureAwait(false);

        return new ResultadoPaginado<ColetaneaItemData>(
            items.AsReadOnly(), total, paginacao.Pagina, paginacao.ItensPorPagina);
    }

    public async Task<ColetaneaDetalheData?> ObterDetalheAsync(
        Guid coletaneaId, Guid usuarioId, CancellationToken ct)
    {
        var coletanea = await _context.Conteudos
            .Where(c => c.Id == coletaneaId
                       && c.UsuarioId == usuarioId // SEG-02
                       && c.Papel == PapelConteudo.Coletanea)
            .Select(c => new
            {
                c.Id,
                c.Titulo,
                c.TipoColetaneaValor,
                c.CriadoEm,
            })
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);

        if (coletanea is null)
            return null;

        // Count total items in collection
        var quantidadeItens = await _context.ConteudoColetaneas
            .CountAsync(cc => cc.ColetaneaId == coletaneaId, ct)
            .ConfigureAwait(false);

        // Calculate progress percentage: count of items with EstadoProgresso == Concluido
        var itensConcluidos = quantidadeItens > 0
            ? await _context.ConteudoColetaneas
                .Where(cc => cc.ColetaneaId == coletaneaId)
                .Join(_context.Conteudos,
                    cc => cc.ConteudoId, c => c.Id,
                    (cc, c) => c.Progresso.Estado)
                .CountAsync(e => e == EstadoProgresso.Concluido, ct)
                .ConfigureAwait(false)
            : 0;

        var progressoPercentual = quantidadeItens > 0
            ? Math.Round((decimal)itensConcluidos / quantidadeItens * 100, 2)
            : 0m;

        // Get primary cover image path
        var imagemCapaCaminho = await _context.Conteudos
            .Where(c => c.Id == coletaneaId && c.UsuarioId == usuarioId)
            .SelectMany(c => c.Imagens)
            .Where(i => i.Principal)
            .Select(i => i.Caminho)
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);

        return new ColetaneaDetalheData(
            coletanea.Id,
            coletanea.Titulo,
            coletanea.TipoColetaneaValor ?? TipoColetanea.Miscelanea,
            quantidadeItens,
            progressoPercentual,
            imagemCapaCaminho,
            coletanea.CriadoEm);
    }
}
