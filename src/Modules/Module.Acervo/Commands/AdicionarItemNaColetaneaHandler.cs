using System.Diagnostics.CodeAnalysis;
using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Core.Enums;
using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Core.Repositorios;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Commands;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes",
    Justification = "Instantiated by MediatR via DI container at runtime.")]
internal sealed class AdicionarItemNaColetaneaHandler : IRequestHandler<AdicionarItemNaColetaneaCommand, Resultado<Unit>>
{
    private readonly IColetaneaRepository _coletaneaRepo;
    private readonly IConteudoRepository _conteudoRepo;

    public AdicionarItemNaColetaneaHandler(
        IColetaneaRepository coletaneaRepo,
        IConteudoRepository conteudoRepo)
    {
        _coletaneaRepo = coletaneaRepo;
        _conteudoRepo = conteudoRepo;
    }

    public async Task<Resultado<Unit>> Handle(AdicionarItemNaColetaneaCommand cmd, CancellationToken ct)
    {
        var coletanea = await _coletaneaRepo.ObterPorIdAsync(cmd.ColetaneaId, cmd.UsuarioId, ct).ConfigureAwait(false);
        if (coletanea is null)
            return Resultado<Unit>.Failure(new Erro("COLETANEA_NAO_ENCONTRADA", "Coletanea nao encontrada"));

        var conteudo = await _conteudoRepo.ObterPorIdAsync(cmd.ConteudoId, cmd.UsuarioId, ct).ConfigureAwait(false);
        if (conteudo is null)
            return Resultado<Unit>.Failure(new Erro("CONTEUDO_NAO_ENCONTRADO", "Conteudo nao encontrado"));

        if (cmd.ColetaneaId == cmd.ConteudoId)
            return Resultado<Unit>.Failure(new Erro("AUTO_REFERENCIA_COLETANEA", "Nao e possivel adicionar uma coletanea a si mesma"));

        var existe = await _coletaneaRepo.ItemExisteNaColetaneaAsync(cmd.ColetaneaId, cmd.ConteudoId, ct).ConfigureAwait(false);
        if (existe)
            return Resultado<Unit>.Failure(new Erro("ITEM_JA_NA_COLETANEA", "Item ja na coletanea"));

        if (conteudo.Papel == PapelConteudo.Coletanea)
        {
            var descendentes = await _coletaneaRepo.ObterDescendentesAsync(cmd.ConteudoId, cmd.UsuarioId, ct).ConfigureAwait(false);
            if (descendentes.Contains(cmd.ColetaneaId))
                return Resultado<Unit>.Failure(new Erro("CICLO_COMPOSICAO", "Ciclo detectado na composicao"));
        }

        var posicao = await _coletaneaRepo.ObterProximaPosicaoAsync(cmd.ColetaneaId, ct).ConfigureAwait(false);
        var item = new ConteudoColetanea
        {
            ColetaneaId = cmd.ColetaneaId,
            ConteudoId = cmd.ConteudoId,
            Posicao = posicao,
            AnotacaoContextual = null,
            AdicionadoEm = DateTimeOffset.UtcNow,
        };

        await _coletaneaRepo.AdicionarItemAsync(item, ct).ConfigureAwait(false);
        return Resultado<Unit>.Success(Unit.Value);
    }
}
