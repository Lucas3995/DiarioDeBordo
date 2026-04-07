using DiarioDeBordo.Core.Primitivos;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Commands;

public sealed record AdicionarImagemCapaCommand(
    Guid ConteudoId,
    Guid UsuarioId,
    string Caminho,
    long TamanhoBytes) : IRequest<Resultado<Unit>>;
