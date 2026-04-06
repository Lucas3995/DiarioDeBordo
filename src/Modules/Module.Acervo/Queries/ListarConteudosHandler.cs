using System.Diagnostics.CodeAnalysis;
using DiarioDeBordo.Core.Consultas;
using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Module.Acervo.DTOs;
using DiarioDeBordo.Module.Shared.Paginacao;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Queries;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes",
    Justification = "Instantiated by MediatR via DI container at runtime.")]
internal sealed class ListarConteudosHandler
    : IRequestHandler<ListarConteudosQuery, Resultado<PaginatedList<ConteudoResumoDto>>>
{
    private readonly IConteudoQueryService _queryService;

    public ListarConteudosHandler(IConteudoQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<Resultado<PaginatedList<ConteudoResumoDto>>> Handle(
        ListarConteudosQuery query, CancellationToken ct)
    {
        var data = await _queryService
            .ListarAsync(query.UsuarioId, query.Paginacao, ct)
            .ConfigureAwait(false);

        var dtos = data.Items
            .Select(d => new ConteudoResumoDto(
                d.Id,
                d.Titulo,
                d.Formato.ToString(),
                d.Papel.ToString(),
                d.CriadoEm,
                d.Classificacao,
                d.Subtipo,
                null))  // Nota not included in list view for performance (detail view has it)
            .ToList()
            .AsReadOnly();

        var lista = new PaginatedList<ConteudoResumoDto>(
            dtos,
            data.TotalItems,
            data.PaginaAtual,
            data.TamanhoPagina);

        return Resultado<PaginatedList<ConteudoResumoDto>>.Success(lista);
    }
}
