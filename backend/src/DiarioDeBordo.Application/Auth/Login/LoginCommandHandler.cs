using DiarioDeBordo.Domain.Auth;
using MediatR;
using DiarioDeBordo.Application.Auth;

namespace DiarioDeBordo.Application.Auth.Login;

/// <summary>
/// Handler CQRS para o comando de login.
///
/// Regras de negócio:
/// 1. Buscar usuário pelo login. Mesmo que não encontrado, o tempo de resposta
///    é equalizado verificando o hash, evitando timing attacks.
/// 2. Se o usuário estiver inativo → credenciais inválidas (mensagem genérica).
/// 3. Se a senha não bater com o hash → credenciais inválidas (mensagem genérica).
/// 4. Se Requer2FA → retornar resposta indicando 2FA necessário (sem emitir token).
/// 5. Sucesso → gerar token JWT e retornar.
///
/// A mensagem de erro é sempre genérica ("Credenciais inválidas") para impedir
/// enumeração de logins existentes.
/// </summary>
public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IAuthenticationService _authenticationService;

    public LoginCommandHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Delegar toda a lógica de autenticação para o serviço extraído.
        return _authenticationService.Authenticate(request, cancellationToken);
    }
}
