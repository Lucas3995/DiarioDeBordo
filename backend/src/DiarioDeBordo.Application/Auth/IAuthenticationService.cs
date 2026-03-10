using DiarioDeBordo.Application.Auth.Login;

namespace DiarioDeBordo.Application.Auth
{
    /// <summary>
    /// Serviço de domínio/aplicação responsável por autenticar um usuário
    /// a partir de um LoginCommand. Encapsula todas as regras de negócio
    /// relacionadas ao processo de login, como verificação de senha,
    /// tratamento de 2FA e geração de token.
    /// </summary>
    public interface IAuthenticationService
    {
        Task<LoginResponse> Authenticate(LoginCommand command, CancellationToken cancellationToken = default);
    }
}