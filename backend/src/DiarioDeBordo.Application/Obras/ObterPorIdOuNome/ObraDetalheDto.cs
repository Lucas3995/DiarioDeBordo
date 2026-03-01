using DiarioDeBordo.Domain.Obras;

namespace DiarioDeBordo.Application.Obras.ObterPorIdOuNome;

/// <summary>
/// DTO com dados atuais da obra para exibição (prévia).
/// </summary>
public sealed record ObraDetalheDto(
    Guid Id,
    string Nome,
    TipoObra Tipo,
    int PosicaoAtual,
    DateTime DataUltimaAtualizacaoPosicao,
    int OrdemPreferencia);
