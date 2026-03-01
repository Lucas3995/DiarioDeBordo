using MediatR;

namespace DiarioDeBordo.Application.Obras.ObterPorIdOuNome;

/// <summary>
/// Query CQRS: retorna os dados atuais de uma obra por id ou nome (para prévia).
/// </summary>
public sealed record GetObraPorIdOuNomeQuery(Guid? Id, string? Nome) : IRequest<ObraDetalheDto?>;
