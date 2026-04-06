using DiarioDeBordo.Core.Enums;
using DiarioDeBordo.Core.Primitivos;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Commands;

/// <summary>
/// Atualiza todos os campos editáveis de um conteúdo existente.
/// CategoriaIds é o conjunto completo desejado — o handler faz o diff com o atual.
/// </summary>
public sealed record AtualizarConteudoCommand(
    Guid Id,
    Guid UsuarioId,
    string Titulo,
    string? Descricao,
    string? Anotacoes,
    decimal? Nota,
    Classificacao? Classificacao,
    FormatoMidia Formato,
    string? Subtipo,
    EstadoProgresso EstadoProgresso,
    string? PosicaoAtual,
    int? TotalEsperadoSessoes,
    IReadOnlyList<Guid> CategoriaIds) : IRequest<Resultado<Unit>>;
