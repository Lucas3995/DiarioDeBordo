using DiarioDeBordo.Core.Primitivos;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Commands;

public sealed record RemoverFonteCommand(
    Guid ConteudoId,
    Guid UsuarioId,
    Guid FonteId) : IRequest<Resultado<Unit>>;
