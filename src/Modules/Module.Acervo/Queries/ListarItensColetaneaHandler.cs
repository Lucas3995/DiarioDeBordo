using System.Diagnostics.CodeAnalysis;
using DiarioDeBordo.Core.Consultas;
using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Module.Acervo.DTOs;
using DiarioDeBordo.Module.Shared.Paginacao;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Queries;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes",
    Justification = "Instantiated by MediatR via DI container at runtime.")]
internal sealed class ListarItensColetaneaHandler
    : IRequestHandler<ListarItensColetaneaQuery, Resultado<PaginatedList<ColetaneaItemDto>>>
{
    private readonly IColetaneaQueryService _queryService;

    public ListarItensColetaneaHandler(IColetaneaQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<Resultado<PaginatedList<ColetaneaItemDto>>> Handle(
        ListarItensColetaneaQuery query, CancellationToken ct)
    {
        var data = await _queryService
            .ListarItensAsync(query.ColetaneaId, query.UsuarioId, query.Paginacao, ct)
            .ConfigureAwait(false);

        var dtos = data.Items
            .Select(d => new ColetaneaItemDto(
                d.ConteudoId,
                d.Titulo,
                d.Posicao,
                d.AnotacaoContextual,
                d.EstadoProgresso.ToString(),
                d.Papel.ToString(),
                d.TipoColetanea?.ToString(),
                d.SubItensContagem,
                d.AdicionadoEm))
            .ToList()
            .AsReadOnly();

        var lista = new PaginatedList<ColetaneaItemDto>(
            dtos,
            data.TotalItems,
            data.PaginaAtual,
            data.TamanhoPagina);

        return Resultado<PaginatedList<ColetaneaItemDto>>.Success(lista);
    }
}
