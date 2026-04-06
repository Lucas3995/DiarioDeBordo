namespace DiarioDeBordo.Core.Primitivos;

/// <summary>
/// Parâmetros de paginação para queries de listagem.
/// Toda listagem de conteúdos usa PaginacaoParams — queries sem paginação são rejeitadas.
/// (Padrões Técnicos v4, seção 3.2; docs/domain/acervo.md — Padrões Transversais)
/// </summary>
public sealed record PaginacaoParams
{
    public int Pagina { get; }
    public int ItensPorPagina { get; }

    /// <summary>Offset calculado — usado por queries que não suportam Skip/Take (ex: SQL raw).</summary>
    public int Offset => (Pagina - 1) * ItensPorPagina;

    /// <summary>Padrão: página 1, 20 itens por página.</summary>
    public static readonly PaginacaoParams Padrao = new(1, 20);

    public PaginacaoParams(int pagina, int itensPorPagina)
    {
        if (pagina < 1)
            throw new ArgumentOutOfRangeException(nameof(pagina), "Página deve ser ≥ 1.");
        if (itensPorPagina < 1 || itensPorPagina > 200)
            throw new ArgumentOutOfRangeException(nameof(itensPorPagina), "ItensPorPagina deve estar entre 1 e 200.");

        Pagina = pagina;
        ItensPorPagina = itensPorPagina;
    }
}

/// <summary>
/// Resultado paginado de uma consulta. Tipo nativo do Core para evitar dependências circulares.
/// Module.Shared pode expor PaginatedList&lt;T&gt; como conveniência.
/// </summary>
public sealed class ResultadoPaginado<T>
{
    public IReadOnlyList<T> Items { get; }
    public int TotalItems { get; }
    public int PaginaAtual { get; }
    public int TamanhoPagina { get; }
    public int TotalPaginas => (int)Math.Ceiling(TotalItems / (double)TamanhoPagina);
    public bool TemPaginaAnterior => PaginaAtual > 1;
    public bool TemProximaPagina => PaginaAtual < TotalPaginas;

    public ResultadoPaginado(IReadOnlyList<T> items, int totalItems, int paginaAtual, int tamanhoPagina)
    {
        Items = items;
        TotalItems = totalItems;
        PaginaAtual = paginaAtual;
        TamanhoPagina = tamanhoPagina;
    }
}

