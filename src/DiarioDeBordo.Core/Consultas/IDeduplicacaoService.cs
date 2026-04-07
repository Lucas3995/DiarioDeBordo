using DiarioDeBordo.Core.Enums;

namespace DiarioDeBordo.Core.Consultas;

/// <summary>
/// Serviço de detecção de duplicatas com dois níveis de confiança (D-07).
/// Alta: URL exata em qualquer fonte. Media: título normalizado.
/// </summary>
public interface IDeduplicacaoService
{
    Task<DuplicataData?> VerificarAsync(
        Guid usuarioId, string titulo, IReadOnlyList<string>? fonteUrls, CancellationToken ct);
}

public sealed record DuplicataData(
    Guid ConteudoId,
    string Titulo,
    DateTimeOffset CriadoEm,
    NivelConfiancaDuplicata Nivel);
