using DiarioDeBordo.Application.Obras;
using DiarioDeBordo.Domain.Obras;
using MediatR;

namespace DiarioDeBordo.Application.Obras.AtualizarPosicao;

/// <summary>
/// Handler do AtualizarPosicaoObraCommand: resolve obra por id ou nome,
/// atualiza posição ou cria nova se não existir.
/// </summary>
public sealed class AtualizarPosicaoObraCommandHandler(IObraEscritaRepository repository)
    : IRequestHandler<AtualizarPosicaoObraCommand, AtualizarPosicaoObraResponse>
{
    public async Task<AtualizarPosicaoObraResponse> Handle(
        AtualizarPosicaoObraCommand request,
        CancellationToken cancellationToken)
    {
        Obra? obra = null;
        if (request.IdObra.HasValue)
            obra = await repository.ObterPorIdAsync(request.IdObra.Value, cancellationToken);
        else if (!string.IsNullOrWhiteSpace(request.NomeObra))
            obra = await repository.ObterPorNomeAsync(request.NomeObra.Trim(), cancellationToken);

        if (obra is not null)
        {
            obra.AtualizarPosicao(request.NovaPosicao, request.DataUltimaAtualizacao);
            await repository.AtualizarAsync(obra, cancellationToken);
            return new AtualizarPosicaoObraResponse(obra.Id, Criada: false);
        }

        if (request.CriarSeNaoExistir && !string.IsNullOrWhiteSpace(request.NomeParaCriar)
            && request.TipoParaCriar.HasValue && request.OrdemPreferenciaParaCriar.HasValue)
        {
            var data = request.DataUltimaAtualizacao ?? DateTime.UtcNow;
            var nova = new Obra(
                request.NomeParaCriar.Trim(),
                request.TipoParaCriar.Value,
                request.NovaPosicao,
                data,
                request.OrdemPreferenciaParaCriar.Value);
            await repository.AdicionarAsync(nova, cancellationToken);
            return new AtualizarPosicaoObraResponse(nova.Id, Criada: true);
        }

        throw new InvalidOperationException("Obra não encontrada e criação não solicitada ou dados para criação incompletos.");
    }
}
