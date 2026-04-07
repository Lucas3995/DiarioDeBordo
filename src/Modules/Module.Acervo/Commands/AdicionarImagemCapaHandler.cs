using System.Diagnostics.CodeAnalysis;
using DiarioDeBordo.Core.Enums;
using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Core.Repositorios;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Commands;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes",
    Justification = "Instantiated by MediatR via DI container at runtime.")]
internal sealed class AdicionarImagemCapaHandler : IRequestHandler<AdicionarImagemCapaCommand, Resultado<Unit>>
{
    private readonly IConteudoRepository _conteudoRepo;

    public AdicionarImagemCapaHandler(IConteudoRepository conteudoRepo)
    {
        _conteudoRepo = conteudoRepo;
    }

    public async Task<Resultado<Unit>> Handle(AdicionarImagemCapaCommand cmd, CancellationToken ct)
    {
        var conteudo = await _conteudoRepo.ObterPorIdAsync(cmd.ConteudoId, cmd.UsuarioId, ct).ConfigureAwait(false);
        if (conteudo is null)
            return Resultado<Unit>.Failure(Erros.NaoEncontrado);

        try
        {
            conteudo.AdicionarImagem(cmd.Caminho, OrigemImagem.Manual, cmd.TamanhoBytes);
        }
        catch (DomainException ex)
        {
            return Resultado<Unit>.Failure(ex.ToErro());
        }

        await _conteudoRepo.AtualizarAsync(conteudo, ct).ConfigureAwait(false);
        return Resultado<Unit>.Success(Unit.Value);
    }
}
