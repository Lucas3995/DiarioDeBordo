using DiarioDeBordo.Application.Auth;
using DiarioDeBordo.Domain.Auth;

namespace DiarioDeBordo.Persistence.Seed;

/// <summary>
/// Seed condicional do usuário admin para ambiente de Development.
///
/// Segurança: a senha "camaradinha@123" é hasheada com BCrypt (work factor 12)
/// na execução do seed. O hash nunca é fixo em código.
///
/// IMPORTANTE: o admin inicia com Requer2FA = false para facilitar o desenvolvimento.
/// Deve-se habilitar o 2FA antes de expor o ambiente para produção.
/// </summary>
public static class UsuariosSeed
{
    public static async Task AplicarAsync(DiarioDeBordoDbContext context, IPasswordHasher passwordHasher)
    {
        if (context.Usuarios.Any())
            return;

        var admin = new Usuario("admin", Perfil.Admin);
        admin.DefinirSenhaHash(passwordHasher.Hash("camaradinha@123"));

        context.Usuarios.Add(admin);
        await context.SaveChangesAsync();
    }
}
