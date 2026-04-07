using System.Diagnostics.CodeAnalysis;
using DiarioDeBordo.Core.Consultas;
using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Module.Acervo.DTOs;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Queries;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes",
    Justification = "Instantiated by MediatR via DI container at runtime.")]
internal sealed class ObterColetaneaDetalheHandler
    : IRequestHandler<ObterColetaneaDetalheQuery, Resultado<ColetaneaDetalheDto>>
{
    private readonly IColetaneaQueryService _queryService;

    public ObterColetaneaDetalheHandler(IColetaneaQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<Resultado<ColetaneaDetalheDto>> Handle(
        ObterColetaneaDetalheQuery query, CancellationToken ct)
    {
        var data = await _queryService
            .ObterDetalheAsync(query.ColetaneaId, query.UsuarioId, ct)
            .ConfigureAwait(false);

        if (data is null)
            return Resultado<ColetaneaDetalheDto>.Failure(new Erro("NAO_ENCONTRADO", "Coletanea nao encontrada"));

        var dto = new ColetaneaDetalheDto(
            data.Id,
            data.Titulo,
            data.TipoColetanea.ToString(),
            data.QuantidadeItens,
            data.ProgressoPercentual,
            data.ImagemCapaCaminho,
            data.CriadoEm);

        return Resultado<ColetaneaDetalheDto>.Success(dto);
    }
}
