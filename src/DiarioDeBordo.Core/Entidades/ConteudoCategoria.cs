namespace DiarioDeBordo.Core.Entidades;

/// <summary>
/// Join entity for M:N relationship between Conteudo and Categoria.
/// Replaces EF Core implicit many-to-many to allow AssociadaEm tracking.
/// </summary>
public sealed class ConteudoCategoria
{
    public Guid ConteudoId { get; init; }
    public Guid CategoriaId { get; init; }
    public DateTimeOffset AssociadaEm { get; init; }
}
