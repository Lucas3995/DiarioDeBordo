using MediatR;

namespace DiarioDeBordo.Application.Obras.Listar;

/// <summary>
/// Query CQRS: retorna lista paginada de obras para a tela de acompanhamento.
/// Apenas leitura — sem efeito colateral.
/// Ordenação default: por OrdemPreferencia (crescente).
/// </summary>
public sealed record GetObrasAcompanhamentoQuery(
    int PageIndex,
    int PageSize) : IRequest<ObrasAcompanhamentoListResponse>;
