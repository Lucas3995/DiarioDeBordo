using DiarioDeBordo.Domain.Auth;

namespace DiarioDeBordo.Infrastructure.Auth;

/// <summary>
/// Implementação de IPasswordHasher usando BCrypt com work factor 12.
/// BCrypt é adequado para hash de senhas por ser resistente a ataques de força bruta:
/// o work factor de 12 garante tempo de hash ~300ms em hardware moderno.
/// </summary>
public sealed class BcryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    public string Hash(string senha) =>
        BCrypt.Net.BCrypt.HashPassword(senha, WorkFactor);

    public bool Verificar(string senha, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(senha, hash);
        }
        catch
        {
            // Hash inválido (ex.: placeholder para timing equalization) → retorna false
            return false;
        }
    }
}
