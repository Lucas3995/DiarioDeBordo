using DiarioDeBordo.Core.Infraestrutura;
using System.Security.Cryptography;
using System.Text;

namespace DiarioDeBordo.Infrastructure.Seguranca;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
internal sealed class ArmazenamentoSeguroWindows : IArmazenamentoSeguro
{
    private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("DiarioDeBordo-v1");

    public async Task ArmazenarAsync(string chave, byte[] valor)
    {
        var encrypted = ProtectedData.Protect(valor, Entropy, DataProtectionScope.CurrentUser);
        var path = GetFilePath(chave);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await File.WriteAllBytesAsync(path, encrypted).ConfigureAwait(false);
    }

    public async Task<byte[]?> RecuperarAsync(string chave)
    {
        var path = GetFilePath(chave);
        if (!File.Exists(path)) return null;
        var encrypted = await File.ReadAllBytesAsync(path).ConfigureAwait(false);
        var decrypted = ProtectedData.Unprotect(encrypted, Entropy, DataProtectionScope.CurrentUser);
        return decrypted;
    }

    public Task RemoverAsync(string chave)
    {
        var path = GetFilePath(chave);
        if (File.Exists(path)) File.Delete(path);
        return Task.CompletedTask;
    }

    private static string GetFilePath(string chave)
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(appData, "DiarioDeBordo", "secure", $"{chave}.dat");
    }
}
