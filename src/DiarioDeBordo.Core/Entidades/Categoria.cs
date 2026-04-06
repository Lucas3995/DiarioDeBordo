namespace DiarioDeBordo.Core.Entidades;

public sealed class Categoria
{
    public Guid Id { get; init; }
    public Guid UsuarioId { get; init; }
    public required string Nome { get; init; }
    public required string NomeNormalizado { get; init; } // always lowercase

    public static Categoria Criar(Guid usuarioId, string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new Primitivos.DomainException("NOME_CATEGORIA_OBRIGATORIO", "Nome de categoria é obrigatório."); // I-11

        return new Categoria
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuarioId,
            Nome = nome.Trim(),
            NomeNormalizado = nome.Trim().ToLowerInvariant(),
        };
    }
}
