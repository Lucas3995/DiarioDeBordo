using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Module.Acervo.DTOs;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Queries;

public sealed record ObterColetaneaDetalheQuery(
    Guid ColetaneaId,
    Guid UsuarioId) : IRequest<Resultado<ColetaneaDetalheDto>>;
