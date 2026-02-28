using DiarioDeBordo.Application.Obras.Listar;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiarioDeBordo.Api.Controllers;

[ApiController]
[Route("api/obras")]
[Authorize]
public sealed class ObrasController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Lista obras paginadas, ordenadas por ordem de preferência.
    /// </summary>
    /// <param name="pageIndex">Índice da página (começa em 0).</param>
    /// <param name="pageSize">Itens por página: 10, 25, 50 ou 100.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    [HttpGet]
    [ProducesResponseType(typeof(ObrasAcompanhamentoListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Listar(
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetObrasAcompanhamentoQuery(pageIndex, pageSize);
        var resultado = await mediator.Send(query, cancellationToken);
        return Ok(resultado);
    }
}
