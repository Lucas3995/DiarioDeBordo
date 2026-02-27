using DiarioDeBordo.Domain.Auth;

namespace DiarioDeBordo.Application.Auth;

/// <summary>
/// Contrato de acesso a dados para a entidade Usuario.
/// Definido na camada Application para que o handler não dependa de EF Core.
/// Implementado em Persistence.
/// </summary>
public interface IUsuarioRepository
{
    Task<Usuario?> BuscarPorLoginAsync(string login, CancellationToken cancellationToken = default);
}
