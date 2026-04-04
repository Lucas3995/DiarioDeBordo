using System.Diagnostics.CodeAnalysis;
using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Core.Repositorios;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Commands;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes",
    Justification = "Instantiated by MediatR via DI container at runtime.")]
internal sealed class ExcluirConteudoHandler : IRequestHandler<ExcluirConteudoCommand, Resultado<Unit>>
{
    // Well-known Guid for "Contém"/"Parte de" system relation type (D-18 sessions)
    private static readonly Guid _contemTipoId = Guid.Parse("10000000-0000-0000-0000-000000000009");

    private readonly IConteudoRepository _conteudoRepo;
    private readonly IRelacaoRepository _relacaoRepo;

    public ExcluirConteudoHandler(IConteudoRepository conteudoRepo, IRelacaoRepository relacaoRepo)
    {
        _conteudoRepo = conteudoRepo;
        _relacaoRepo = relacaoRepo;
    }

    public async Task<Resultado<Unit>> Handle(ExcluirConteudoCommand cmd, CancellationToken ct)
    {
        var conteudo = await _conteudoRepo.ObterPorIdAsync(cmd.Id, cmd.UsuarioId, ct).ConfigureAwait(false);
        if (conteudo is null)
            return Resultado<Unit>.Failure(Erros.NaoEncontrado);

        // 1. Find and delete child sessions (IsFilho=true, linked via "Contém" relation)
        var filhosIds = await _conteudoRepo
            .ListarFilhosAsync(cmd.Id, cmd.UsuarioId, _contemTipoId, ct)
            .ConfigureAwait(false);

        foreach (var filhoId in filhosIds)
        {
            // Remove child's relations (including inverse "Parte de")
            var relacoesFilho = await _relacaoRepo
                .ListarPorConteudoAsync(filhoId, cmd.UsuarioId, ct)
                .ConfigureAwait(false);
            foreach (var rel in relacoesFilho)
            {
                await _relacaoRepo.RemoverParAsync(rel.Id, cmd.UsuarioId, ct).ConfigureAwait(false);
            }

            // Remove child's categories
            await _conteudoRepo.RemoverTodasCategoriasAsync(filhoId, ct).ConfigureAwait(false);

            // Remove child content
            await _conteudoRepo.RemoverAsync(filhoId, cmd.UsuarioId, ct).ConfigureAwait(false);
        }

        // 2. Remove all relations for this content (forward + inverse via ParId)
        var relacoes = await _relacaoRepo
            .ListarPorConteudoAsync(cmd.Id, cmd.UsuarioId, ct)
            .ConfigureAwait(false);
        foreach (var rel in relacoes)
        {
            await _relacaoRepo.RemoverParAsync(rel.Id, cmd.UsuarioId, ct).ConfigureAwait(false);
        }

        // 3. Remove this content's category associations
        await _conteudoRepo.RemoverTodasCategoriasAsync(cmd.Id, ct).ConfigureAwait(false);

        // 4. Remove the content itself
        await _conteudoRepo.RemoverAsync(cmd.Id, cmd.UsuarioId, ct).ConfigureAwait(false);

        return Resultado<Unit>.Success(Unit.Value);
    }
}
