using DiarioDeBordo.Domain.Obras;
using MediatR;

namespace DiarioDeBordo.Application.Obras.AtualizarPosicao;

/// <summary>
/// Command CQRS: atualiza a posição de uma obra (por id ou nome) ou cria nova se não existir.
/// </summary>
public sealed record AtualizarPosicaoObraCommand(
    Guid? IdObra,
    string? NomeObra,
    int NovaPosicao,
    DateTime? DataUltimaAtualizacao,
    bool CriarSeNaoExistir,
    string? NomeParaCriar = null,
    TipoObra? TipoParaCriar = null,
    int? OrdemPreferenciaParaCriar = null) : IRequest<AtualizarPosicaoObraResponse>;
