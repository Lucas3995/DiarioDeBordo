using System.Diagnostics.CodeAnalysis;
using DiarioDeBordo.Core.Repositorios;
using DiarioDeBordo.Module.Acervo.DTOs;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Queries;

/// <summary>Busca tipos de relação para autocomplete (sistemas + criados pelo usuário).</summary>
public sealed record BuscarTiposRelacaoQuery(
    Guid UsuarioId,
    string Prefixo) : IRequest<IReadOnlyList<TipoRelacaoDto>>;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes",
    Justification = "Instantiated by MediatR via DI container at runtime.")]
internal sealed class BuscarTiposRelacaoHandler : IRequestHandler<BuscarTiposRelacaoQuery, IReadOnlyList<TipoRelacaoDto>>
{
    private readonly ITipoRelacaoRepository _tipoRelacaoRepo;

    public BuscarTiposRelacaoHandler(ITipoRelacaoRepository tipoRelacaoRepo)
    {
        _tipoRelacaoRepo = tipoRelacaoRepo;
    }

    public async Task<IReadOnlyList<TipoRelacaoDto>> Handle(BuscarTiposRelacaoQuery query, CancellationToken ct)
    {
        var tipos = await _tipoRelacaoRepo
            .ListarComAutocompletarAsync(query.UsuarioId, query.Prefixo, ct)
            .ConfigureAwait(false);

        return tipos
            .Select(t => new TipoRelacaoDto(t.Id, t.Nome, t.NomeInverso, t.IsSistema))
            .ToList()
            .AsReadOnly();
    }
}
