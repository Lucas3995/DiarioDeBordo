using DiarioDeBordo.Domain.Obras;

namespace DiarioDeBordo.Application.Obras.Listar;

/// <summary>
/// Mapeia entidade Obra para DTO da listagem de acompanhamento.
/// Responsabilidade única: projeção Obra → ObraAcompanhamentoListItemDto.
/// </summary>
public static class ObraAcompanhamentoListMapper
{
    public static ObraAcompanhamentoListItemDto Map(Obra obra)
    {
        var proximaInfo = MapProximaInfo(obra);
        return new ObraAcompanhamentoListItemDto(
            Id: obra.Id.ToString(),
            Nome: obra.Nome,
            Tipo: obra.Tipo.ToString().ToLowerInvariant(),
            UltimaAtualizacaoPosicao: obra.DataUltimaAtualizacaoPosicao,
            PosicaoAtual: obra.PosicaoAtual,
            ProximaInfo: proximaInfo,
            OrdemPreferencia: obra.OrdemPreferencia);
    }

    private static ProximaInfoDto? MapProximaInfo(Obra obra)
    {
        return obra.ProximaInfoTipo switch
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
    }
}
