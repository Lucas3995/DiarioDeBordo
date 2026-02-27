using DiarioDeBordo.Application.Echo;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DiarioDeBordo.Api.Controllers;

/// <summary>
/// Endpoint de exemplo para validar o pipeline CQRS (Command + FluentValidation + Handler).
/// </summary>
[ApiController]
[Route("[controller]")]
public sealed class EchoController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Ecoa a mensagem enviada após validação.
    /// Valida o fluxo: Api → ValidationBehavior → Handler → resposta.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EchoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Eco([FromBody] EchoCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }
}
