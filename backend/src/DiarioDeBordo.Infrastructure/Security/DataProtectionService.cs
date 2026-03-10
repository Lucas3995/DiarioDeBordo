using System.Security.Cryptography;
using System.Text;
using DiarioDeBordo.Domain.Common;
using Microsoft.Extensions.Configuration;

namespace DiarioDeBordo.Infrastructure.Security;

/// <summary>
/// Criptografia de dados sensíveis em nível de campo (LGPD Art. 46) usando AES-GCM.
/// A chave é lida de configuração (DataProtection:Key) e deve ser gerenciada via vault/secrets externo.
/// Formato do output: Base64(nonce ‖ ciphertext ‖ tag).
/// </summary>
public sealed class DataProtectionService : IDataProtectionService
{
    private const int NonceSize = 12;
    private const int TagSize = 16;
    private readonly byte[] _key;

    public DataProtectionService(IConfiguration configuration)
    {
        var keyBase64 = configuration["DataProtection:Key"]
            ?? throw new InvalidOperationException(
                "DataProtection:Key não configurada. Defina uma chave AES-256 em Base64 via variável de ambiente ou vault.");

        _key = Convert.FromBase64String(keyBase64);

        if (_key.Length != 32)
            throw new InvalidOperationException("DataProtection:Key deve ser uma chave AES-256 (32 bytes em Base64).");
    }

    public string Criptografar(string valorEmClaro)
    {
        var plaintext = Encoding.UTF8.GetBytes(valorEmClaro);
        var nonce = new byte[NonceSize];
        RandomNumberGenerator.Fill(nonce);

        var ciphertext = new byte[plaintext.Length];
        var tag = new byte[TagSize];

        using var aes = new AesGcm(_key, TagSize);
        aes.Encrypt(nonce, plaintext, ciphertext, tag);

        var result = new byte[NonceSize + ciphertext.Length + TagSize];
        nonce.CopyTo(result, 0);
        ciphertext.CopyTo(result, NonceSize);
        tag.CopyTo(result, NonceSize + ciphertext.Length);

        return Convert.ToBase64String(result);
    }

    public string Descriptografar(string valorCriptografado)
    {
        var combined = Convert.FromBase64String(valorCriptografado);

        if (combined.Length < NonceSize + TagSize)
            throw new CryptographicException("Dados criptografados inválidos: tamanho insuficiente.");

        var nonce = combined.AsSpan(0, NonceSize);
        var ciphertextLength = combined.Length - NonceSize - TagSize;
        var ciphertext = combined.AsSpan(NonceSize, ciphertextLength);
        var tag = combined.AsSpan(NonceSize + ciphertextLength, TagSize);

        var plaintext = new byte[ciphertextLength];

        using var aes = new AesGcm(_key, TagSize);
        aes.Decrypt(nonce, ciphertext, tag, plaintext);

        return Encoding.UTF8.GetString(plaintext);
    }
}
