using DiarioDeBordo.Core.Primitivos;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Commands;

/// <summary>Remove uma relação bidirecional (ambos os lados do par).</summary>
public sealed record RemoverRelacaoCommand(Guid RelacaoId, Guid UsuarioId) : IRequest<Resultado<Unit>>;
