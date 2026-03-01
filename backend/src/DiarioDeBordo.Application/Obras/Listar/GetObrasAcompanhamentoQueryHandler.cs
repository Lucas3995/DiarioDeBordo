using DiarioDeBordo.Domain.Obras;
using MediatR;

namespace DiarioDeBordo.Application.Obras.Listar;

/// <summary>
/// Handler para GetObrasAcompanhamentoQuery.
/// Orquestra repositório e mapeamento; a projeção Obra → DTO fica em ObraAcompanhamentoListMapper.
/// </summary>
public sealed class GetObrasAcompanhamentoQueryHandler(IObraLeituraRepository repository)
    : IRequestHandler<GetObrasAcompanhamentoQuery, ObrasAcompanhamentoListResponse>
{
    public async Task<ObrasAcompanhamentoListResponse> Handle(
        GetObrasAcompanhamentoQuery request,
        CancellationToken cancellationToken)
    {
        var (itens, totalCount) = await repository.ListarPaginadoAsync(
            request.PageIndex,
            request.PageSize,
            cancellationToken);

        var dtos = itens.Select(ObraAcompanhamentoListMapper.Map).ToList();

        return new ObrasAcompanhamentoListResponse(dtos, totalCount);
    }
}
