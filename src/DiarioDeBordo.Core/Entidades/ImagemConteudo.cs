using DiarioDeBordo.Core.Enums;

namespace DiarioDeBordo.Core.Entidades;

public sealed class ImagemConteudo
{
    public Guid Id { get; init; }
    public Guid ConteudoId { get; init; }
    public OrigemImagem OrigemTipo { get; init; }
    public required string Caminho { get; init; }
    public bool Principal { get; set; }
}
