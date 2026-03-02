using DiarioDeBordo.Domain.Obras;

namespace DiarioDeBordo.Api.Models;

/// <summary>
/// Request body para PATCH /api/obras/posicao.
/// </summary>
public sealed class AtualizarPosicaoObraRequest
{
    public Guid? IdObra { get; set; }
    public string? NomeObra { get; set; }
    public int NovaPosicao { get; set; }
    public DateTime? DataUltimaAtualizacao { get; set; }
    public bool CriarSeNaoExistir { get; set; }
    public string? NomeParaCriar { get; set; }
    public TipoObra? TipoParaCriar { get; set; }
    public int? OrdemPreferenciaParaCriar { get; set; }
}
