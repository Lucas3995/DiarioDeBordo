using System.Diagnostics.CodeAnalysis;
using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Core.Repositorios;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Commands;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes",
    Justification = "Instantiated by MediatR via DI container at runtime.")]
internal sealed class CriarRelacaoHandler : IRequestHandler<CriarRelacaoCommand, Resultado<Guid>>
{
    private readonly IConteudoRepository _conteudoRepo;
    private readonly IRelacaoRepository _relacaoRepo;
    private readonly ITipoRelacaoRepository _tipoRelacaoRepo;

    public CriarRelacaoHandler(
        IConteudoRepository conteudoRepo,
        IRelacaoRepository relacaoRepo,
        ITipoRelacaoRepository tipoRelacaoRepo)
    {
        _conteudoRepo = conteudoRepo;
        _relacaoRepo = relacaoRepo;
        _tipoRelacaoRepo = tipoRelacaoRepo;
    }

    public async Task<Resultado<Guid>> Handle(CriarRelacaoCommand cmd, CancellationToken ct)
    {
        // Self-reference validation
        if (cmd.ConteudoOrigemId == cmd.ConteudoDestinoId)
            return Resultado<Guid>.Failure(Erros.AutoReferenciaProibida);

        // Validate both contents exist and belong to user (SEG-02)
        var origem = await _conteudoRepo
            .ObterPorIdAsync(cmd.ConteudoOrigemId, cmd.UsuarioId, ct)
            .ConfigureAwait(false);
        if (origem is null)
            return Resultado<Guid>.Failure(Erros.NaoEncontrado);

        var destino = await _conteudoRepo
            .ObterPorIdAsync(cmd.ConteudoDestinoId, cmd.UsuarioId, ct)
            .ConfigureAwait(false);
        if (destino is null)
            return Resultado<Guid>.Failure(Erros.NaoEncontrado);

        // Get or create relation type (case-insensitive dedup)
        var tipo = await _tipoRelacaoRepo
            .ObterOuCriarAsync(cmd.UsuarioId, cmd.NomeTipoRelacao, cmd.NomeInverso, ct)
            .ConfigureAwait(false);

        // Duplicate relation check
        var jaExiste = await _relacaoRepo
            .ExisteAsync(cmd.ConteudoOrigemId, cmd.ConteudoDestinoId, tipo.Id, cmd.UsuarioId, ct)
            .ConfigureAwait(false);
        if (jaExiste)
            return Resultado<Guid>.Failure(Erros.RelacaoDuplicada);

        // Create linked pair (D-15 — bidirectional, always two rows)
        var forwardId = Guid.NewGuid();
        var inverseId = Guid.NewGuid();
        var agora = DateTimeOffset.UtcNow;

        var forward = new Relacao
        {
            Id = forwardId,
            ConteudoOrigemId = cmd.ConteudoOrigemId,
            ConteudoDestinoId = cmd.ConteudoDestinoId,
            TipoRelacaoId = tipo.Id,
            IsInversa = false,
            ParId = inverseId,
            UsuarioId = cmd.UsuarioId,
            CriadoEm = agora,
        };

        var inverse = new Relacao
        {
            Id = inverseId,
            ConteudoOrigemId = cmd.ConteudoDestinoId,
            ConteudoDestinoId = cmd.ConteudoOrigemId,
            TipoRelacaoId = tipo.Id,
            IsInversa = true,
            ParId = forwardId,
            UsuarioId = cmd.UsuarioId,
            CriadoEm = agora,
        };

        await _relacaoRepo.AdicionarParAsync(forward, inverse, ct).ConfigureAwait(false);

        return Resultado<Guid>.Success(forward.Id);
    }
}
