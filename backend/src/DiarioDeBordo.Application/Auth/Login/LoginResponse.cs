namespace DiarioDeBordo.Application.Auth.Login;

/// <summary>
/// Resposta do comando de login.
///
/// - Sucesso=true: token JWT emitido (Requer2FA=false).
/// - Requer2FA=true: credenciais válidas mas segundo fator ainda necessário;
///   Token=null até que o 2FA seja verificado (fase futura).
/// - Sucesso=false, Requer2FA=false: credenciais inválidas ou usuário inativo.
///   Erro contém mensagem genérica (sem revelar se login existe — proteção contra enumeração).
/// </summary>
public sealed record LoginResponse(
    string? Token,
    DateTime? ExpiresAt,
    bool Requer2FA,
    bool Sucesso,
    string? Erro);
