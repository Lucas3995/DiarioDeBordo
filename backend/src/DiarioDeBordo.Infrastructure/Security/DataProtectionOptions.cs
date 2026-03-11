namespace DiarioDeBordo.Infrastructure.Security
{
    /// <summary>
    /// Opções para <see cref="DataProtectionService"/>, carregadas de configuração
    /// pela seção "DataProtection".
    /// </summary>
    public sealed record DataProtectionOptions(string Key);
}