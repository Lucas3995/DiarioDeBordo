using DiarioDeBordo.Core.Primitivos;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Commands;

public sealed record AdicionarItemNaColetaneaCommand(
    Guid ColetaneaId,
    Guid ConteudoId,
    Guid UsuarioId) : IRequest<Resultado<Unit>>;
