namespace DiarioDeBordo.Domain.Common;

/// <summary>
/// Contrato para criptografia de dados sensíveis em nível de campo.
/// Garante defesa em profundidade (LGPD Art. 46): mesmo com acesso ao banco,
/// dados sensíveis permanecem ilegíveis sem a chave gerenciada externamente.
/// 
/// Implementação concreta em Infrastructure; chaves gerenciadas via vault/secrets externos.
/// </summary>
public interface IDataProtectionService
{
    /// <summary>Criptografa um valor para armazenamento em campo sensível.</summary>
    string Criptografar(string valorEmClaro);

    /// <summary>Descriptografa um campo sensível para uso em memória.</summary>
    string Descriptografar(string valorCriptografado);
}
