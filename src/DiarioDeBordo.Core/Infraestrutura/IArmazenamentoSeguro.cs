namespace DiarioDeBordo.Core.Infraestrutura;

/// <summary>
/// Armazenamento seguro de segredos usando mecanismo do SO.
/// Windows: DPAPI. Linux: libsecret/secret-tool.
/// (Padrões Técnicos v4, seção 4.3)
/// </summary>
public interface IArmazenamentoSeguro
{
    Task ArmazenarAsync(string chave, byte[] valor);
    Task<byte[]?> RecuperarAsync(string chave);
    Task RemoverAsync(string chave);
}
