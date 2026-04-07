using DiarioDeBordo.Core.Primitivos;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Commands;

public sealed record AtualizarAnotacaoContextualCommand(
    Guid ColetaneaId,
    Guid ConteudoId,
    Guid UsuarioId,
    string? AnotacaoContextual) : IRequest<Resultado<Unit>>;
