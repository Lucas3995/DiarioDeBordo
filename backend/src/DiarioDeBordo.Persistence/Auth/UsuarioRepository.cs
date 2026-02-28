using DiarioDeBordo.Application.Auth;
using DiarioDeBordo.Domain.Auth;
using Microsoft.EntityFrameworkCore;

namespace DiarioDeBordo.Persistence.Auth;

/// <summary>
/// Implementação concreta de IUsuarioRepository usando EF Core.
/// Usa AsNoTracking para operações de leitura.
/// </summary>
public sealed class UsuarioRepository : IUsuarioRepository
{
    private readonly DiarioDeBordoDbContext _context;

    public UsuarioRepository(DiarioDeBordoDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> BuscarPorLoginAsync(string login, CancellationToken cancellationToken = default) =>
        await _context.Usuarios
                      .AsNoTracking()
                      .FirstOrDefaultAsync(u => u.Login == login, cancellationToken);
}
