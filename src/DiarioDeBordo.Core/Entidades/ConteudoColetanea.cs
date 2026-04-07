namespace DiarioDeBordo.Core.Entidades;

/// <summary>
/// Entidade de junção entre Conteudo (coletânea) e Conteudo (item) com payload.
/// Composite PK: (ColetaneaId, ConteudoId). Um conteúdo aparece no máximo uma vez por coletânea.
/// ACE-08: AnotacaoContextual pertence à relação, não ao conteúdo.
/// </summary>
public sealed class ConteudoColetanea
{
    public Guid ColetaneaId { get; init; }
    public Guid ConteudoId { get; init; }
    public int Posicao { get; set; }
    public string? AnotacaoContextual { get; set; }
    public DateTimeOffset AdicionadoEm { get; init; }
}
