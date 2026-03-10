using DiarioDeBordo.Application.Auth.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DiarioDeBordo.Api.Controllers;

/// <summary>
/// Endpoints de autenticação.
/// POST /api/auth/login é público (sem [Authorize]) pois é a porta de entrada para obter o token.
/// </summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Autentica um usuário com login e senha.
    /// Retorna 200 com token JWT se bem-sucedido, 401 se credenciais inválidas,
    /// 400 se a requisição falhar validação.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        if (!result.Sucesso && !result.Requer2FA)
            return Unauthorized(new { result.Erro });

        return Ok(result);
    }
}
