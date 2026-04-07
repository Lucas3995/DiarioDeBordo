using DiarioDeBordo.Core.Primitivos;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Commands;

public sealed record RemoverImagemCapaCommand(
    Guid ConteudoId,
    Guid UsuarioId,
    Guid ImagemId) : IRequest<Resultado<Unit>>;
