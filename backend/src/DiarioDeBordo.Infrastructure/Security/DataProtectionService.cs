using DiarioDeBordo.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace DiarioDeBordo.Infrastructure.Security;

/// <summary>
/// Stub/placeholder da criptografia de dados sensíveis em nível de campo (LGPD Art. 46).
/// 
/// TODO (etapa futura): substituir por implementação real usando:
///   - ASP.NET Core Data Protection API, ou
///   - AWS KMS / Azure Key Vault, ou
///   - AES-GCM com chaves armazenadas em vault externo.
/// 
/// As chaves NUNCA devem ser armazenadas no mesmo repositório dos dados.
/// </summary>
public sealed class DataProtectionService(ILogger<DataProtectionService> logger)
    : IDataProtectionService
{
    public string Criptografar(string valorEmClaro)
    {
        logger.LogWarning(
            "DataProtectionService: criptografia de campo não implementada. " +
            "Use em produção apenas após configurar a implementação real com vault externo.");

        return valorEmClaro;
    }

    public string Descriptografar(string valorCriptografado)
    {
        logger.LogWarning(
            "DataProtectionService: descriptografia de campo não implementada. " +
            "Use em produção apenas após configurar a implementação real com vault externo.");

        return valorCriptografado;
    }
}
