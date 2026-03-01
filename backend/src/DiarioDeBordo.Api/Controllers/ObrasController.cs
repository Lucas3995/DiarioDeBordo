using DiarioDeBordo.Api.Models;
using DiarioDeBordo.Application.Obras.AtualizarPosicao;
using DiarioDeBordo.Application.Obras.Listar;
using DiarioDeBordo.Application.Obras.ObterPorIdOuNome;
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

    /// <summary>
    /// Obtém uma obra por id (para prévia).
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ObraDetalheDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetObraPorIdOuNomeQuery(Id: id, Nome: null);
        var resultado = await mediator.Send(query, cancellationToken);
        return resultado is null ? NotFound() : Ok(resultado);
    }

    /// <summary>
    /// Obtém uma obra por nome (para prévia).
    /// </summary>
    [HttpGet("por-nome")]
    [ProducesResponseType(typeof(ObraDetalheDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorNome([FromQuery] string nome, CancellationToken cancellationToken = default)
    {
        var query = new GetObraPorIdOuNomeQuery(Id: null, Nome: nome);
        var resultado = await mediator.Send(query, cancellationToken);
        return resultado is null ? NotFound() : Ok(resultado);
    }

    /// <summary>
    /// Atualiza a posição de uma obra (por id ou nome) ou cria nova se não existir.
    /// </summary>
    [HttpPatch("posicao")]
    [ProducesResponseType(typeof(AtualizarPosicaoObraResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AtualizarPosicao(
        [FromBody] AtualizarPosicaoObraRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new AtualizarPosicaoObraCommand(
            request.IdObra,
            request.NomeObra,
            request.NovaPosicao,
            request.DataUltimaAtualizacao,
            request.CriarSeNaoExistir,
            request.NomeParaCriar,
            request.TipoParaCriar,
            request.OrdemPreferenciaParaCriar);
        try
        {
            var resultado = await mediator.Send(command, cancellationToken);
            return Ok(resultado);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }
}
