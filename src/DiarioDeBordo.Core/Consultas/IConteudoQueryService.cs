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
    Task<ResultadoPaginado<SessaoData>> ListarSessoesAsync(Guid conteudoPaiId, Guid usuarioId, PaginacaoParams paginacao, CancellationToken ct);
}

public sealed record ConteudoResumoData(
    Guid Id,
    string Titulo,
    FormatoMidia Formato,
    PapelConteudo Papel,
    DateTimeOffset CriadoEm,
    Classificacao? Classificacao,
    string? Subtipo);

public sealed record ConteudoDetalheData(
    Guid Id,
    string Titulo,
    string? Descricao,
    string? Anotacoes,
    decimal? Nota,
    FormatoMidia Formato,
    PapelConteudo Papel,
    DateTimeOffset CriadoEm,
    Classificacao? Classificacao,
    bool IsFilho,
    int? TotalEsperadoSessoes,
    string? Subtipo,
    EstadoProgresso EstadoProgresso,
    string? PosicaoAtual,
    IReadOnlyList<CategoriaData> Categorias,
    IReadOnlyList<RelacaoData> Relacoes,
    IReadOnlyList<SessaoData> Sessoes,
    int SessoesContagem);

/// <summary>Categoria associada ao conteúdo. IsAutomatica = gerada por importação (D-11).</summary>
public sealed record CategoriaData(Guid Id, string Nome, bool IsAutomatica);

/// <summary>Relação de um conteúdo com outro, incluindo o tipo (nome no sentido exibido).</summary>
public sealed record RelacaoData(Guid Id, Guid ConteudoDestinoId, string TituloDestino, string NomeTipo, bool IsInversa);

/// <summary>Sessão filho exibida na timeline cronológica (D-17/D-18).</summary>
public sealed record SessaoData(
    Guid Id,
    string Titulo,
    DateTimeOffset CriadoEm,
    Classificacao? Classificacao,
    decimal? Nota,
    string? Anotacoes);
