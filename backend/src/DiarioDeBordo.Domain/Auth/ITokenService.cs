namespace DiarioDeBordo.Domain.Auth;

/// <summary>
/// Contrato para geração de tokens JWT.
/// Definido na camada Domain; implementado em Infrastructure
/// para isolar detalhes de JWT da lógica de negócio.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Gera um token JWT assinado para o usuário autenticado.
    /// </summary>
    /// <returns>Token JWT serializado e sua data de expiração.</returns>
    (string Token, DateTime ExpiresAt) GerarToken(Usuario usuario);
}
