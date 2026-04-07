using System.Diagnostics.CodeAnalysis;
using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Core.Repositorios;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Commands;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes",
    Justification = "Instantiated by MediatR via DI container at runtime.")]
internal sealed class AtualizarAnotacaoContextualHandler : IRequestHandler<AtualizarAnotacaoContextualCommand, Resultado<Unit>>
{
    private readonly IColetaneaRepository _coletaneaRepo;

    public AtualizarAnotacaoContextualHandler(IColetaneaRepository coletaneaRepo)
    {
        _coletaneaRepo = coletaneaRepo;
    }

    public async Task<Resultado<Unit>> Handle(AtualizarAnotacaoContextualCommand cmd, CancellationToken ct)
    {
        var item = await _coletaneaRepo.ObterItemAsync(cmd.ColetaneaId, cmd.ConteudoId, cmd.UsuarioId, ct).ConfigureAwait(false);
        if (item is null)
            return Resultado<Unit>.Failure(Erros.NaoEncontrado);

        item.AnotacaoContextual = cmd.AnotacaoContextual;
        await _coletaneaRepo.AtualizarItemAsync(item, ct).ConfigureAwait(false);
        return Resultado<Unit>.Success(Unit.Value);
    }
}
