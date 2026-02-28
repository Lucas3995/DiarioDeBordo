using MediatR;

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
    private const string ErroGenerico = "Credenciais inválidas.";

    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;

    public LoginCommandHandler(
        IUsuarioRepository usuarioRepository,
        ITokenService tokenService,
        IPasswordHasher passwordHasher)
    {
        _usuarioRepository = usuarioRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
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
