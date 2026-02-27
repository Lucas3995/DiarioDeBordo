using DiarioDeBordo.Application.Status;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DiarioDeBordo.Api.Controllers;

/// <summary>
/// Retorna informações de status da API — útil para health check de deploy e monitoramento.
/// </summary>
[ApiController]
[Route("[controller]")]
public sealed class StatusController(IMediator mediator) : ControllerBase
{
    /// <summary>Obtém o status atual da API, versão e ambiente.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(StatusResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterStatus(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetStatusQuery(), cancellationToken);
        return Ok(response);
    }
}
