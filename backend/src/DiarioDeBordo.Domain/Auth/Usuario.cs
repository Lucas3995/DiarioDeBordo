using DiarioDeBordo.Domain.Common;

namespace DiarioDeBordo.Domain.Auth;

/// <summary>
/// Entidade de usuário do sistema.
///
/// Dados pessoais (LGPD Art. 5º): Login identifica diretamente uma pessoa natural.
/// SenhaHash nunca é exposto em DTOs, logs ou respostas da API.
///
/// Arquitetura 2FA: o campo Requer2FA permite exigir segundo fator antes de emitir
/// o JWT. Para ambiente de desenvolvimento o admin inicia com Requer2FA = false;
/// deve ser habilitado antes de produção com implementação TOTP (fase futura).
/// </summary>
public sealed class Usuario : Entity
{
    public string Login { get; private set; } = string.Empty;
    public string SenhaHash { get; private set; } = string.Empty;
    public bool Ativo { get; private set; }
    public bool Requer2FA { get; private set; }
    public Perfil Perfil { get; private set; }

    private Usuario() { }

    public Usuario(string login, Perfil perfil)
    {
        ValidarLogin(login);

        Login = login.Trim();
        Perfil = perfil;
        Ativo = true;
        Requer2FA = false;
    }

    /// <summary>
    /// Define o hash da senha. Deve ser chamado apenas com o resultado de IPasswordHasher.Hash().
    /// Nunca armazenar a senha em texto claro.
    /// </summary>
    public void DefinirSenhaHash(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new ArgumentException("Hash da senha não pode ser vazio.", nameof(hash));

        SenhaHash = hash;
    }

    public void Ativar() => Ativo = true;

    public void Desativar() => Ativo = false;

    public void HabilitarSegundoFator() => Requer2FA = true;

    public void DesabilitarSegundoFator() => Requer2FA = false;

    private static void ValidarLogin(string login)
    {
        if (string.IsNullOrWhiteSpace(login))
            throw new ArgumentException("Login não pode ser vazio.", nameof(login));

        if (login.Trim().Length > 100)
            throw new ArgumentException("Login deve ter no máximo 100 caracteres.", nameof(login));
    }
}
