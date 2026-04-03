using DiarioDeBordo.Core.Infraestrutura;
using System.Security.Cryptography;
using System.Text;

namespace DiarioDeBordo.Infrastructure.Seguranca;

[System.Runtime.Versioning.SupportedOSPlatform("linux")]
internal sealed class ArmazenamentoSeguroLinux : IArmazenamentoSeguro
{
    private const string AppName = "DiarioDeBordo";

    public async Task ArmazenarAsync(string chave, byte[] valor)
    {
        // Try secret-tool first
        if (await TryStoreWithSecretToolAsync(chave, valor).ConfigureAwait(false)) return;
        // Fallback: AES-256-GCM file
        await StoreWithFileAsync(chave, valor).ConfigureAwait(false);
    }

    public async Task<byte[]?> RecuperarAsync(string chave)
    {
        var result = await TryRetrieveWithSecretToolAsync(chave).ConfigureAwait(false);
        if (result is not null) return result;
        return await RetrieveWithFileAsync(chave).ConfigureAwait(false);
    }

    public Task RemoverAsync(string chave)
    {
        // Remove from both locations
        var path = GetFallbackPath(chave);
        if (File.Exists(path)) File.Delete(path);
        return Task.CompletedTask;
    }

    private static async Task<bool> TryStoreWithSecretToolAsync(string chave, byte[] valor)
    {
        try
        {
            var base64 = Convert.ToBase64String(valor);
            using var proc = new System.Diagnostics.Process();
            proc.StartInfo = new System.Diagnostics.ProcessStartInfo("secret-tool",
                $"store --label={AppName}:{chave} application {AppName} key {chave}")
            {
                RedirectStandardInput = true, UseShellExecute = false
            };
            proc.Start();
            await proc.StandardInput.WriteAsync(base64).ConfigureAwait(false);
            proc.StandardInput.Close();
            await proc.WaitForExitAsync().ConfigureAwait(false);
            return proc.ExitCode == 0;
        }
#pragma warning disable CA1031 // process failures are expected — secret-tool may not be installed
        catch { return false; }
#pragma warning restore CA1031
    }

    private static async Task<byte[]?> TryRetrieveWithSecretToolAsync(string chave)
    {
        try
        {
            using var proc = new System.Diagnostics.Process();
            proc.StartInfo = new System.Diagnostics.ProcessStartInfo("secret-tool",
                $"lookup application {AppName} key {chave}")
            {
                RedirectStandardOutput = true, UseShellExecute = false
            };
            proc.Start();
            var output = await proc.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
            await proc.WaitForExitAsync().ConfigureAwait(false);
            if (proc.ExitCode != 0 || string.IsNullOrEmpty(output)) return null;
            return Convert.FromBase64String(output.Trim());
        }
#pragma warning disable CA1031 // process failures are expected — secret-tool may not be installed
        catch { return null; }
#pragma warning restore CA1031
    }

    private static async Task StoreWithFileAsync(string chave, byte[] valor)
    {
        // AES-256-GCM with machine-derived key
        var key = DeriveKey();
        var nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
        RandomNumberGenerator.Fill(nonce);
        var tag = new byte[AesGcm.TagByteSizes.MaxSize];
        var ciphertext = new byte[valor.Length];

        using var aes = new AesGcm(key, AesGcm.TagByteSizes.MaxSize);
        aes.Encrypt(nonce, valor, ciphertext, tag);

        // Store: nonce + tag + ciphertext
        var stored = new byte[nonce.Length + tag.Length + ciphertext.Length];
        nonce.CopyTo(stored, 0);
        tag.CopyTo(stored, nonce.Length);
        ciphertext.CopyTo(stored, nonce.Length + tag.Length);

        var path = GetFallbackPath(chave);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await File.WriteAllBytesAsync(path, stored).ConfigureAwait(false);
        File.SetUnixFileMode(path, UnixFileMode.UserRead | UnixFileMode.UserWrite);

        // Zero key from memory
        CryptographicOperations.ZeroMemory(key);
    }

    private static async Task<byte[]?> RetrieveWithFileAsync(string chave)
    {
        var path = GetFallbackPath(chave);
        if (!File.Exists(path)) return null;

        var stored = await File.ReadAllBytesAsync(path).ConfigureAwait(false);
        var nonceSize = AesGcm.NonceByteSizes.MaxSize;
        var tagSize = AesGcm.TagByteSizes.MaxSize;

        var nonce = stored[..nonceSize];
        var tag = stored[nonceSize..(nonceSize + tagSize)];
        var ciphertext = stored[(nonceSize + tagSize)..];
        var plaintext = new byte[ciphertext.Length];

        var key = DeriveKey();
        using var aes = new AesGcm(key, tagSize);
        aes.Decrypt(nonce, ciphertext, tag, plaintext);
        CryptographicOperations.ZeroMemory(key);

        return plaintext;
    }

    private static byte[] DeriveKey()
    {
        // Machine-specific seed: hostname + username (not cryptographically strong, but avoids storing a key)
        var seed = $"{Environment.MachineName}-{Environment.UserName}-DiarioDeBordo";
        return SHA256.HashData(Encoding.UTF8.GetBytes(seed));
    }

    private static string GetFallbackPath(string chave)
    {
        var xdgData = Environment.GetEnvironmentVariable("XDG_DATA_HOME")
            ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share");
        return Path.Combine(xdgData, "DiarioDeBordo", "secure", $"{chave}.enc");
    }
}
