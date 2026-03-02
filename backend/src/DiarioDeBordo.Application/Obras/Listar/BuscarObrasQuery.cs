using MediatR;

namespace DiarioDeBordo.Application.Obras.Listar;

/// <summary>
/// Query CQRS: busca obras por termo no nome para autocomplete (Demanda 4).
/// Retorna lista reduzida (id, nome) com até Limit itens.
/// </summary>
public sealed record BuscarObrasQuery(string? Q, int Limit = 10)
    : IRequest<IReadOnlyList<ObraBuscaItemDto>>;
