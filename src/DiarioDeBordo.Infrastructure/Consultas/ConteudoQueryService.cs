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
    // Well-known Guid for "Contém"/"Parte de" system relation type (D-18 sessions)
    private static readonly Guid _contemTipoId = Guid.Parse("10000000-0000-0000-0000-000000000009");

    private readonly DiarioDeBordoDbContext _context;

    public ConteudoQueryService(DiarioDeBordoDbContext context)
    {
        _context = context;
    }

    public async Task<ResultadoPaginado<ConteudoResumoData>> ListarAsync(
        Guid usuarioId, PaginacaoParams paginacao, CancellationToken ct)
    {
        var total = await _context.Conteudos
            .CountAsync(c => c.UsuarioId == usuarioId && !c.IsFilho, ct)
            .ConfigureAwait(false);

        var items = await _context.Conteudos
            .Where(c => c.UsuarioId == usuarioId && !c.IsFilho) // SEG-02 + D-19: filhos ocultos
            .OrderByDescending(c => c.CriadoEm)
            .Skip(paginacao.Offset)
            .Take(paginacao.ItensPorPagina)
            .Select(c => new ConteudoResumoData(c.Id, c.Titulo, c.Formato, c.Papel, c.CriadoEm, c.Classificacao, c.Subtipo))
            .ToListAsync(ct)
            .ConfigureAwait(false);

        return new ResultadoPaginado<ConteudoResumoData>(
            items.AsReadOnly(), total, paginacao.Pagina, paginacao.ItensPorPagina);
    }

    public async Task<ConteudoDetalheData?> ObterAsync(Guid id, Guid usuarioId, CancellationToken ct)
    {
        // Main content query
        var conteudo = await _context.Conteudos
            .Where(c => c.Id == id && c.UsuarioId == usuarioId) // SEG-02: usuarioId mandatory
            .Select(c => new
            {
                c.Id, c.Titulo, c.Descricao, c.Anotacoes, c.Nota, c.Formato, c.Papel,
                c.CriadoEm, c.Classificacao, c.IsFilho, c.TotalEsperadoSessoes, c.Subtipo,
                c.Progresso.Estado, c.Progresso.PosicaoAtual,
            })
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);

        if (conteudo is null)
            return null;

        // Categories query (via ConteudoCategoria join)
        var categorias = await _context.ConteudoCategorias
            .Where(cc => cc.ConteudoId == id)
            .Join(_context.Categorias, cc => cc.CategoriaId, cat => cat.Id,
                (cc, cat) => new CategoriaData(cat.Id, cat.Nome, false))
            .ToListAsync(ct)
            .ConfigureAwait(false);

        // Relations query (non-session relations only — exclude Contém/Parte de)
        var relacoes = await _context.Relacoes
            .Where(r => r.ConteudoOrigemId == id && r.UsuarioId == usuarioId
                        && r.TipoRelacaoId != _contemTipoId)
            .Join(_context.TipoRelacoes, r => r.TipoRelacaoId, t => t.Id,
                (r, t) => new { r, t })
            .Join(_context.Conteudos, rt => rt.r.ConteudoDestinoId, c => c.Id,
                (rt, c) => new RelacaoData(
                    rt.r.Id,
                    c.Id,
                    c.Titulo,
                    rt.r.IsInversa ? rt.t.NomeInverso : rt.t.Nome,
                    rt.r.IsInversa))
            .ToListAsync(ct)
            .ConfigureAwait(false);

        // Sessions query (child contents linked by "Contém" relation, ordered by CriadoEm DESC)
        // Capped at 50 most recent — SC-4 compliance
        var sessoesIds = await _context.Relacoes
            .Where(r => r.ConteudoOrigemId == id && r.UsuarioId == usuarioId
                        && r.TipoRelacaoId == _contemTipoId && !r.IsInversa)
            .Select(r => r.ConteudoDestinoId)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        var sessoes = await _context.Conteudos
            .Where(c => sessoesIds.Contains(c.Id))
            .OrderByDescending(c => c.CriadoEm)
            .Take(50)
            .Select(c => new SessaoData(c.Id, c.Titulo, c.CriadoEm, c.Classificacao, c.Nota, c.Anotacoes))
            .ToListAsync(ct)
            .ConfigureAwait(false);

        return new ConteudoDetalheData(
            conteudo.Id,
            conteudo.Titulo,
            conteudo.Descricao,
            conteudo.Anotacoes,
            conteudo.Nota,
            conteudo.Formato,
            conteudo.Papel,
            conteudo.CriadoEm,
            conteudo.Classificacao,
            conteudo.IsFilho,
            conteudo.TotalEsperadoSessoes,
            conteudo.Subtipo,
            conteudo.Estado,
            conteudo.PosicaoAtual,
            categorias.AsReadOnly(),
            relacoes.AsReadOnly(),
            sessoes.AsReadOnly(),
            sessoesIds.Count);
    }

    public async Task<ResultadoPaginado<SessaoData>> ListarSessoesAsync(
        Guid conteudoPaiId, Guid usuarioId, PaginacaoParams paginacao, CancellationToken ct)
    {
        // Find child session IDs via "Contém" relations (D-18)
        var sessoesIds = await _context.Relacoes
            .Where(r => r.ConteudoOrigemId == conteudoPaiId
                        && r.UsuarioId == usuarioId
                        && r.TipoRelacaoId == _contemTipoId
                        && !r.IsInversa)
            .Select(r => r.ConteudoDestinoId)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        var total = sessoesIds.Count;

        var sessoes = await _context.Conteudos
            .Where(c => sessoesIds.Contains(c.Id))
            .OrderBy(c => c.CriadoEm)
            .Skip(paginacao.Offset)
            .Take(paginacao.ItensPorPagina)
            .Select(c => new SessaoData(c.Id, c.Titulo, c.CriadoEm, c.Classificacao, c.Nota, c.Anotacoes))
            .ToListAsync(ct)
            .ConfigureAwait(false);

        return new ResultadoPaginado<SessaoData>(
            sessoes.AsReadOnly(), total, paginacao.Pagina, paginacao.ItensPorPagina);
    }
}
