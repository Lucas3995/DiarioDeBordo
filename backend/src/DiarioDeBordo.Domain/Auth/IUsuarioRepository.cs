namespace DiarioDeBordo.Domain.Auth;

/// <summary>
/// Contrato de acesso a dados para a entidade Usuario.
/// Definido na camada Domain para que handlers não dependam de EF Core.
/// Implementado em Persistence.
/// </summary>
public interface IUsuarioRepository
{
    Task<Usuario?> BuscarPorLoginAsync(string login, CancellationToken cancellationToken = default);
}
