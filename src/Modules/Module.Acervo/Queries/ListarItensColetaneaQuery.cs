using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Module.Acervo.DTOs;
using DiarioDeBordo.Module.Shared.Paginacao;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Queries;

public sealed record ListarItensColetaneaQuery(
    Guid ColetaneaId,
    Guid UsuarioId,
    PaginacaoParams Paginacao) : IRequest<Resultado<PaginatedList<ColetaneaItemDto>>>;
