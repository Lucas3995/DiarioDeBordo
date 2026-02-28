namespace DiarioDeBordo.Application.Obras.Listar;

/// <summary>
/// DTO de item da listagem paginada de obras para acompanhamento.
/// Contém apenas os campos necessários para a tela de listagem (CQRS — projeção mínima).
///
/// Datas em UTC. O frontend aplica o fuso Brasil (UTC-3 / America/Sao_Paulo) e formata
/// no padrão dd/MM/yyyy.
///
/// ProximaInfo segue discriminador compatível com o tipo TypeScript do frontend:
///   { tipo: "dias_ate_proxima", dias: N } ou { tipo: "partes_ja_publicadas", quantidade: N }
/// </summary>
public sealed record ObraAcompanhamentoListItemDto(
    string Id,
    string Nome,
    string Tipo,
    DateTime UltimaAtualizacaoPosicao,
    int PosicaoAtual,
    ProximaInfoDto? ProximaInfo,
    int OrdemPreferencia);

/// <summary>Previsão consolidada da próxima parte (opcional).</summary>
public sealed record ProximaInfoDto(string Tipo, int? Dias, int? Quantidade);

/// <summary>Resposta paginada da listagem de obras.</summary>
public sealed record ObrasAcompanhamentoListResponse(
    IReadOnlyList<ObraAcompanhamentoListItemDto> Items,
    int TotalCount);
