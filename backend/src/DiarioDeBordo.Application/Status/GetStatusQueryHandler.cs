using MediatR;
using Microsoft.Extensions.Hosting;

namespace DiarioDeBordo.Application.Status;

/// <summary>
/// Handler para GetStatusQuery. Retorna metadados do servidor sem tocar no banco.
/// Não contém lógica de domínio — apenas orquestração de informação de ambiente.
/// </summary>
public sealed class GetStatusQueryHandler(IHostEnvironment environment)
    : IRequestHandler<GetStatusQuery, StatusResponse>
{
    public Task<StatusResponse> Handle(GetStatusQuery request, CancellationToken cancellationToken)
    {
        var response = new StatusResponse(
            Versao: "1.0.0",
            Ambiente: environment.EnvironmentName,
            HoraServidor: DateTime.UtcNow);

        return Task.FromResult(response);
    }
}
