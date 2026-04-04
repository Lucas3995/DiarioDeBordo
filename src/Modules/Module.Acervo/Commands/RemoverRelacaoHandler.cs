using System.Diagnostics.CodeAnalysis;
using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Core.Repositorios;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Commands;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes",
    Justification = "Instantiated by MediatR via DI container at runtime.")]
internal sealed class RemoverRelacaoHandler : IRequestHandler<RemoverRelacaoCommand, Resultado<Unit>>
{
    private readonly IRelacaoRepository _relacaoRepo;

    public RemoverRelacaoHandler(IRelacaoRepository relacaoRepo)
    {
        _relacaoRepo = relacaoRepo;
    }

    public async Task<Resultado<Unit>> Handle(RemoverRelacaoCommand cmd, CancellationToken ct)
    {
        // RemoverParAsync handles both sides (forward + inverse via ParId)
        // SEG-02: usuarioId filter applied inside repository
        await _relacaoRepo.RemoverParAsync(cmd.RelacaoId, cmd.UsuarioId, ct).ConfigureAwait(false);
        return Resultado<Unit>.Success(Unit.Value);
    }
}
