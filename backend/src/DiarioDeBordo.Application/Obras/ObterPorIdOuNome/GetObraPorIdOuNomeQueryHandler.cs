using DiarioDeBordo.Domain.Obras;
using MediatR;

namespace DiarioDeBordo.Application.Obras.ObterPorIdOuNome;

/// <summary>
/// Handler da GetObraPorIdOuNomeQuery: obtém obra por id ou nome e retorna DTO para prévia.
/// </summary>
public sealed class GetObraPorIdOuNomeQueryHandler(IObraEscritaRepository repository)
    : IRequestHandler<GetObraPorIdOuNomeQuery, ObraDetalheDto?>
{
    public async Task<ObraDetalheDto?> Handle(
        GetObraPorIdOuNomeQuery request,
        CancellationToken cancellationToken)
    {
        Obra? obra = null;
        if (request.Id.HasValue)
            obra = await repository.ObterPorIdAsync(request.Id.Value, cancellationToken);
        else if (!string.IsNullOrWhiteSpace(request.Nome))
            obra = await repository.ObterPorNomeAsync(request.Nome.Trim(), cancellationToken);

        if (obra is null)
            return null;

        return new ObraDetalheDto(
            obra.Id,
            obra.Nome,
            obra.Tipo,
            obra.PosicaoAtual,
            obra.DataUltimaAtualizacaoPosicao,
            obra.OrdemPreferencia);
    }
}
