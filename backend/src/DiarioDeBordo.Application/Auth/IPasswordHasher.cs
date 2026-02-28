namespace DiarioDeBordo.Application.Auth;

/// <summary>
/// Contrato para hash e verificação de senhas.
/// Definido na camada Application para que o handler não dependa da biblioteca
/// de hash concreta (BCrypt, PBKDF2, etc.). Implementado em Infrastructure.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>Gera um hash seguro da senha em texto claro.</summary>
    string Hash(string senha);

    /// <summary>Verifica se uma senha em texto claro corresponde ao hash armazenado.</summary>
    bool Verificar(string senha, string hash);
}
