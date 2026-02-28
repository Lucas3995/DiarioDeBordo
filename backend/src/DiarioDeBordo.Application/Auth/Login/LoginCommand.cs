using MediatR;

namespace DiarioDeBordo.Application.Auth.Login;

/// <summary>
/// Comando CQRS para autenticar um usuário com login e senha.
/// Segue o princípio CQRS: esta operação altera o estado da sessão (emite token)
/// e não retorna dados de negócio além do necessário para a autenticação.
/// </summary>
public sealed record LoginCommand(string Login, string Senha) : IRequest<LoginResponse>;
