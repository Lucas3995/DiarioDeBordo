using DiarioDeBordo.Core.Primitivos;
using Microsoft.EntityFrameworkCore;

namespace DiarioDeBordo.Module.Shared.Paginacao;

/// <summary>
/// Lista paginada com metadados de paginação.
/// Toda listagem de conteúdos usa esta classe — nunca retornar List&lt;T&gt; diretamente de serviços.
/// (Padrões Técnicos v4, seção 3.2)
/// </summary>
public sealed class PaginatedList<T>
{
    /// <summary>Itens da página atual.</summary>
    public IReadOnlyList<T> Items { get; }

    /// <summary>Total de itens em todas as páginas (resultado do COUNT).</summary>
    public int TotalItems { get; }

    public int PaginaAtual { get; }
    public int TamanhoPagina { get; }

    public int TotalPaginas => TamanhoPagina > 0
        ? (int)Math.Ceiling(TotalItems / (double)TamanhoPagina)
        : 0;

    public bool TemPaginaAnterior => PaginaAtual > 1;
    public bool TemProximaPagina => PaginaAtual < TotalPaginas;

    public PaginatedList(IReadOnlyList<T> items, int totalItems, int paginaAtual, int tamanhoPagina)
    {
        Items = items;
        TotalItems = totalItems;
        PaginaAtual = paginaAtual;
        TamanhoPagina = tamanhoPagina;
    }

    /// <summary>
    /// Cria uma PaginatedList executando COUNT + SELECT paginado via EF Core.
    /// O IQueryable ainda não deve ter sido executado (sem ToList/ToArray/etc).
    /// </summary>
#pragma warning disable CA1000 // Factory methods on generic types are intentional design (query pagination)
    public static async Task<PaginatedList<T>> CriarAsync(
        IQueryable<T> source,
        PaginacaoParams paginacao,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(paginacao);

        var total = await source.CountAsync(ct).ConfigureAwait(false);
        var items = await source
            .Skip(paginacao.Offset)
            .Take(paginacao.ItensPorPagina)
            .ToListAsync(ct).ConfigureAwait(false);

        return new PaginatedList<T>(items.AsReadOnly(), total, paginacao.Pagina, paginacao.ItensPorPagina);
    }

    /// <summary>
    /// Cria uma PaginatedList a partir de uma lista in-memory (útil para testes e queries já executadas).
    /// </summary>
    public static PaginatedList<T> FromList(
        IReadOnlyList<T> allItems,
        PaginacaoParams paginacao)
    {
        ArgumentNullException.ThrowIfNull(allItems);
        ArgumentNullException.ThrowIfNull(paginacao);

        var total = allItems.Count;
        var items = allItems
            .Skip(paginacao.Offset)
            .Take(paginacao.ItensPorPagina)
            .ToList()
            .AsReadOnly();

        return new PaginatedList<T>(items, total, paginacao.Pagina, paginacao.ItensPorPagina);
    }

    /// <summary>Lista vazia — para estados iniciais e casos de zero resultados.</summary>
    public static PaginatedList<T> Vazia(PaginacaoParams? paginacao = null)
    {
        var p = paginacao ?? PaginacaoParams.Padrao;
        return new PaginatedList<T>([], 0, p.Pagina, p.ItensPorPagina);
    }
#pragma warning restore CA1000
}
