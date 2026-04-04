using System.Diagnostics.CodeAnalysis;
using DiarioDeBordo.Core.Consultas;
using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Module.Acervo.DTOs;
using DiarioDeBordo.Module.Shared.Paginacao;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Queries;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes",
    Justification = "Instantiated by MediatR via DI container at runtime.")]
internal sealed class ListarSessoesHandler
    : IRequestHandler<ListarSessoesQuery, Resultado<PaginatedList<SessaoDto>>>
{
    private readonly IConteudoQueryService _queryService;

    public ListarSessoesHandler(IConteudoQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<Resultado<PaginatedList<SessaoDto>>> Handle(
        ListarSessoesQuery query, CancellationToken ct)
    {
        var data = await _queryService
            .ListarSessoesAsync(query.ConteudoPaiId, query.UsuarioId, query.Paginacao, ct)
            .ConfigureAwait(false);

        var dtos = data.Items
            .Select(s => new SessaoDto(s.Id, s.Titulo, s.CriadoEm, s.Classificacao, s.Nota, s.Anotacoes))
            .ToList()
            .AsReadOnly();

        var lista = new PaginatedList<SessaoDto>(dtos, data.TotalItems, data.PaginaAtual, data.TamanhoPagina);
        return Resultado<PaginatedList<SessaoDto>>.Success(lista);
    }
}
