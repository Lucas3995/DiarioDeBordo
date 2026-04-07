using DiarioDeBordo.Core.Primitivos;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Commands;

public sealed record ReordenarFontesCommand(
    Guid ConteudoId,
    Guid UsuarioId,
    IReadOnlyList<Guid> FonteIdsOrdenados) : IRequest<Resultado<Unit>>;
