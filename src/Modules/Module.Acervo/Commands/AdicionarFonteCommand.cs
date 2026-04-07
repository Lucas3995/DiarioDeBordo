using DiarioDeBordo.Core.Enums;
using DiarioDeBordo.Core.Primitivos;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Commands;

public sealed record AdicionarFonteCommand(
    Guid ConteudoId,
    Guid UsuarioId,
    TipoFonte Tipo,
    string Valor,
    string? Plataforma = null) : IRequest<Resultado<Unit>>;
