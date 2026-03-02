using DiarioDeBordo.Domain.Obras;
using MediatR;

namespace DiarioDeBordo.Application.Obras.Listar;

/// <summary>
/// Handler para BuscarObrasQuery: delega ao repositório e mapeia para DTO reduzido.
/// </summary>
public sealed class BuscarObrasQueryHandler(IObraLeituraRepository repository)
    : IRequestHandler<BuscarObrasQuery, IReadOnlyList<ObraBuscaItemDto>>
{
    public async Task<IReadOnlyList<ObraBuscaItemDto>> Handle(
        BuscarObrasQuery request,
        CancellationToken cancellationToken)
    {
        var limit = request.Limit <= 0 ? 10 : Math.Min(request.Limit, 50);
        var itens = await repository.BuscarPorNomeAsync(request.Q, limit, cancellationToken);
        return itens
            .Select(o => new ObraBuscaItemDto(o.Id.ToString(), o.Nome))
            .ToList();
    }
}
