using DiarioDeBordo.Core.Enums;
using DiarioDeBordo.Core.Primitivos;

namespace DiarioDeBordo.Core.Consultas;

/// <summary>
/// Contrato de consulta somente-leitura para Conteudo.
/// Separado de IConteudoRepository (CQRS — segregação de query vs. command).
/// </summary>
public interface IConteudoQueryService
{
    Task<ResultadoPaginado<ConteudoResumoData>> ListarAsync(Guid usuarioId, PaginacaoParams paginacao, CancellationToken ct);
    Task<ConteudoDetalheData?> ObterAsync(Guid id, Guid usuarioId, CancellationToken ct);
}

public sealed record ConteudoResumoData(
    Guid Id,
    string Titulo,
    FormatoMidia Formato,
    PapelConteudo Papel,
    DateTimeOffset CriadoEm);

public sealed record ConteudoDetalheData(
    Guid Id,
    string Titulo,
    string? Descricao,
    string? Anotacoes,
    decimal? Nota,
    FormatoMidia Formato,
    PapelConteudo Papel,
    DateTimeOffset CriadoEm);
