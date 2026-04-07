using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Core.Enums;
using DiarioDeBordo.Module.Acervo.DTOs;
using DiarioDeBordo.Module.Shared.Paginacao;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Queries;

/// <summary>
/// Lista conteúdos do usuário, paginados, ordenados por data de adição decrescente.
/// REGRA: sempre usa PaginacaoParams — sem listagens infinitas ou sem paginação.
/// </summary>
public sealed record ListarConteudosQuery(
    Guid UsuarioId,
    PaginacaoParams Paginacao,
    PapelConteudo? PapelFiltro = null) : IRequest<Resultado<PaginatedList<ConteudoResumoDto>>>;

/// <summary>Obtém o detalhe de um conteúdo específico do acervo do usuário.</summary>
public sealed record ObterConteudoQuery(
    Guid Id,
    Guid UsuarioId) : IRequest<Resultado<ConteudoDetalheDto>>;

/// <summary>Lista as sessões (filhos) de um conteúdo pai, paginadas e ordenadas cronologicamente (D-18).</summary>
public sealed record ListarSessoesQuery(
    Guid ConteudoPaiId,
    Guid UsuarioId,
    PaginacaoParams Paginacao) : IRequest<Resultado<PaginatedList<SessaoDto>>>;
