using DiarioDeBordo.Core.Primitivos;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Commands;

/// <summary>Remove um conteúdo e todas as suas relações e sessões filhas.</summary>
public sealed record ExcluirConteudoCommand(Guid Id, Guid UsuarioId) : IRequest<Resultado<Unit>>;
