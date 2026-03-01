using MediatR;

namespace DiarioDeBordo.Application.Status;

/// <summary>
/// Handler para GetStatusQuery. Retorna metadados do servidor sem tocar no banco.
/// Não contém lógica de domínio — apenas orquestração de informação de ambiente.
/// </summary>
public sealed class GetStatusQueryHandler(IAmbiente ambiente)
    : IRequestHandler<GetStatusQuery, StatusResponse>
{
    public Task<StatusResponse> Handle(GetStatusQuery request, CancellationToken cancellationToken)
    {
        var response = new StatusResponse(
            Versao: "1.0.0",
            Ambiente: ambiente.NomeAmbiente,
            HoraServidor: DateTime.UtcNow);

        return Task.FromResult(response);
    }
}
