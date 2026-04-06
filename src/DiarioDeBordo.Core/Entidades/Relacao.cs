namespace DiarioDeBordo.Core.Entidades;

/// <summary>
/// Relação direcional entre dois conteúdos.
/// Bidirecionalidade via two-row pattern: forward + inverse com ParId ligando os dois.
/// Validation (auto-reference, duplicates) handled in commands handlers.
/// </summary>
public sealed class Relacao
{
    public Guid Id { get; init; }
    public Guid ConteudoOrigemId { get; init; }
    public Guid ConteudoDestinoId { get; init; }
    public Guid TipoRelacaoId { get; init; }

    /// <summary>True para a linha inversa do par bidirecional.</summary>
    public bool IsInversa { get; init; }

    /// <summary>ID do parceiro bidirecional. Forward.ParId == Inverse.Id, Inverse.ParId == Forward.Id.</summary>
    public Guid? ParId { get; init; }

    public Guid UsuarioId { get; init; }
    public DateTimeOffset CriadoEm { get; init; }
}
