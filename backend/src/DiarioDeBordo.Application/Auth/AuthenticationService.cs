using DiarioDeBordo.Domain.Auth;
using DiarioDeBordo.Application.Auth.Login;

namespace DiarioDeBordo.Application.Auth
{
    /// <summary>
    /// Implementação de <see cref="IAuthenticationService"/> que contém
    /// a lógica original que estava em <see cref="LoginCommandHandler"/>.
    /// A extração reduz a responsabilidade do handler e facilita testes
    /// unitários isolados dessa lógica.
    /// </summary>
    public sealed class AuthenticationService : IAuthenticationService
    {
        private const string ErroGenerico = "Credenciais inválidas.";

        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;

        public AuthenticationService(
            IUsuarioRepository usuarioRepository,
            ITokenService tokenService,
            IPasswordHasher passwordHasher)
        {
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        public async Task<LoginResponse> Authenticate(LoginCommand request, CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioRepository.BuscarPorLoginAsync(request.Login, cancellationToken);

            // Usuário não encontrado — executar verificação com hash fictício para equalizar tempo
            // e evitar que um atacante distinga "login inexistente" de "senha errada" por timing.
            if (usuario is null)
            {
                _passwordHasher.Verificar(request.Senha, "$2a$12$invalidhashplaceholderXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
                return Falha();
            }

            if (!usuario.Ativo)
                return Falha();

            var senhaValida = _passwordHasher.Verificar(request.Senha, usuario.SenhaHash);
            if (!senhaValida)
                return Falha();

            if (usuario.Requer2FA)
            {
                return new LoginResponse(
                    Token: null,
                    ExpiresAt: null,
                    Requer2FA: true,
                    Sucesso: false,
                    Erro: null);
            }

            var (token, expiresAt) = _tokenService.GerarToken(usuario);

            return new LoginResponse(
                Token: token,
                ExpiresAt: expiresAt,
                Requer2FA: false,
                Sucesso: true,
                Erro: null);
        }

        private static LoginResponse Falha() =>
            new(Token: null, ExpiresAt: null, Requer2FA: false, Sucesso: false, Erro: ErroGenerico);
    }
}