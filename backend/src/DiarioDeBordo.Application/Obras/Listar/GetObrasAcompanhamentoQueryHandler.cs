using DiarioDeBordo.Domain.Obras;
using MediatR;

namespace DiarioDeBordo.Application.Obras.Listar;

/// <summary>
/// Handler para GetObrasAcompanhamentoQuery.
/// Depende apenas de IObraLeituraRepository (definido em Application),
/// garantindo que a camada Application não conheça EF Core nem DbContext.
/// </summary>
public sealed class GetObrasAcompanhamentoQueryHandler(IObraLeituraRepository repository)
    : IRequestHandler<GetObrasAcompanhamentoQuery, ObrasAcompanhamentoListResponse>
{
    public async Task<ObrasAcompanhamentoListResponse> Handle(
        GetObrasAcompanhamentoQuery request,
        CancellationToken cancellationToken)
    {
        var (itens, totalCount) = await repository.ListarPaginadoAsync(
            request.PageIndex,
            request.PageSize,
            cancellationToken);

        var dtos = itens.Select(MapearParaDto).ToList();

        return new ObrasAcompanhamentoListResponse(dtos, totalCount);
    }

    private static ObraAcompanhamentoListItemDto MapearParaDto(Obra obra)
    {
        ProximaInfoDto? proximaInfo = obra.ProximaInfoTipo switch
        {
            ProximaInfoTipo.DiasAteProxima => new ProximaInfoDto(
                Tipo: "dias_ate_proxima",
                Dias: obra.DiasAteProximaParte,
                Quantidade: null),

            ProximaInfoTipo.PartesJaPublicadas => new ProximaInfoDto(
                Tipo: "partes_ja_publicadas",
                Dias: null,
                Quantidade: obra.PartesJaPublicadas),

            _ => null
        };

        return new ObraAcompanhamentoListItemDto(
            Id: obra.Id.ToString(),
            Nome: obra.Nome,
            Tipo: obra.Tipo.ToString().ToLowerInvariant(),
            UltimaAtualizacaoPosicao: obra.DataUltimaAtualizacaoPosicao,
            PosicaoAtual: obra.PosicaoAtual,
            ProximaInfo: proximaInfo,
            OrdemPreferencia: obra.OrdemPreferencia);
    }
}
