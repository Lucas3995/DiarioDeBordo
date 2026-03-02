namespace DiarioDeBordo.Application.Obras.Listar;

/// <summary>
/// DTO reduzido para autocomplete (Demanda 4 — GET /api/obras/buscar).
/// Apenas Id e Nome para resposta rápida.
/// </summary>
public sealed record ObraBuscaItemDto(string Id, string Nome);
