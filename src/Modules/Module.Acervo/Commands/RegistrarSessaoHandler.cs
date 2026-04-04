using System.Diagnostics.CodeAnalysis;
using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Core.Repositorios;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Commands;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes",
    Justification = "Instantiated by MediatR via DI container at runtime.")]
internal sealed class RegistrarSessaoHandler : IRequestHandler<RegistrarSessaoCommand, Resultado<Guid>>
{
    // Well-known Guid for "Contém"/"Parte de" system relation type (D-18)
    private static readonly Guid _contemTipoId = Guid.Parse("10000000-0000-0000-0000-000000000009");

    private readonly IConteudoRepository _conteudoRepo;
    private readonly IRelacaoRepository _relacaoRepo;

    public RegistrarSessaoHandler(IConteudoRepository conteudoRepo, IRelacaoRepository relacaoRepo)
    {
        _conteudoRepo = conteudoRepo;
        _relacaoRepo = relacaoRepo;
    }

    public async Task<Resultado<Guid>> Handle(RegistrarSessaoCommand cmd, CancellationToken ct)
    {
        // Validate parent exists and belongs to user (SEG-02)
        var pai = await _conteudoRepo
            .ObterPorIdAsync(cmd.ConteudoPaiId, cmd.UsuarioId, ct)
            .ConfigureAwait(false);
        if (pai is null)
            return Resultado<Guid>.Failure(Erros.NaoEncontrado);

        // Create child session with IsFilho=true (D-17)
        Conteudo filho;
        try
        {
            filho = Conteudo.CriarComoFilho(cmd.UsuarioId, cmd.Titulo, cmd.Formato, cmd.DataConsumo);
        }
        catch (DomainException ex)
        {
            return Resultado<Guid>.Failure(ex.ToErro());
        }

        if (cmd.Anotacoes is not null)
            filho.Anotacoes = cmd.Anotacoes;

        if (cmd.Nota.HasValue)
        {
            try { filho.DefinirNota(cmd.Nota.Value); }
            catch (DomainException ex) { return Resultado<Guid>.Failure(ex.ToErro()); }
        }

        if (cmd.Classificacao.HasValue)
            filho.DefinirClassificacao(cmd.Classificacao.Value);

        // Persist child
        await _conteudoRepo.AdicionarAsync(filho, ct).ConfigureAwait(false);

        // Create bidirectional "Contém"/"Parte de" relation (D-18)
        var forwardId = Guid.NewGuid();
        var inverseId = Guid.NewGuid();
        var agora = DateTimeOffset.UtcNow;

        var forward = new Relacao
        {
            Id = forwardId,
            ConteudoOrigemId = cmd.ConteudoPaiId,
            ConteudoDestinoId = filho.Id,
            TipoRelacaoId = _contemTipoId,
            IsInversa = false,
            ParId = inverseId,
            UsuarioId = cmd.UsuarioId,
            CriadoEm = agora,
        };

        var inverse = new Relacao
        {
            Id = inverseId,
            ConteudoOrigemId = filho.Id,
            ConteudoDestinoId = cmd.ConteudoPaiId,
            TipoRelacaoId = _contemTipoId,
            IsInversa = true,
            ParId = forwardId,
            UsuarioId = cmd.UsuarioId,
            CriadoEm = agora,
        };

        await _relacaoRepo.AdicionarParAsync(forward, inverse, ct).ConfigureAwait(false);

        return Resultado<Guid>.Success(filho.Id);
    }
}
