using DiarioDeBordo.Core.Enums;
using DiarioDeBordo.Core.Primitivos;

namespace DiarioDeBordo.Core.Consultas;

/// <summary>Read-side queries for collection data. CQRS separation from IColetaneaRepository.</summary>
public interface IColetaneaQueryService
{
    Task<ResultadoPaginado<ColetaneaItemData>> ListarItensAsync(
        Guid coletaneaId, Guid usuarioId, PaginacaoParams paginacao, CancellationToken ct);
    Task<ColetaneaDetalheData?> ObterDetalheAsync(Guid coletaneaId, Guid usuarioId, CancellationToken ct);
}

public sealed record ColetaneaItemData(
    Guid ConteudoId,
    string Titulo,
    int Posicao,
    string? AnotacaoContextual,
    EstadoProgresso EstadoProgresso,
    PapelConteudo Papel,
    TipoColetanea? TipoColetanea,
    int? SubItensContagem,
    DateTimeOffset AdicionadoEm);

public sealed record ColetaneaDetalheData(
    Guid Id,
    string Titulo,
    TipoColetanea TipoColetanea,
    int QuantidadeItens,
    decimal ProgressoPercentual,
    string? ImagemCapaCaminho,
    DateTimeOffset CriadoEm);
