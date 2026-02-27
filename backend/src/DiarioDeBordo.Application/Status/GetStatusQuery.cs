using MediatR;

namespace DiarioDeBordo.Application.Status;

/// <summary>
/// Query CQRS: apenas lê e retorna estado — sem efeito colateral.
/// Demonstra o pipeline MediatR funcionando na camada Application.
/// </summary>
public sealed record GetStatusQuery : IRequest<StatusResponse>;

public sealed record StatusResponse(
    string Versao,
    string Ambiente,
    DateTime HoraServidor);
