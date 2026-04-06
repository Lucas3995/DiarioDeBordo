using System.Diagnostics.CodeAnalysis;
using DiarioDeBordo.Core.Consultas;
using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Module.Acervo.DTOs;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Queries;

/// <summary>Busca conteúdos para autocomplete no formulário de relação (exclui filhos e o próprio conteúdo).</summary>
public sealed record BuscarConteudosQuery(
    Guid UsuarioId,
    string Prefixo,
    Guid ExcluirId) : IRequest<IReadOnlyList<ConteudoResumoDto>>;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes",
    Justification = "Instantiated by MediatR via DI container at runtime.")]
internal sealed class BuscarConteudosHandler : IRequestHandler<BuscarConteudosQuery, IReadOnlyList<ConteudoResumoDto>>
{
    private readonly IConteudoQueryService _queryService;

    public BuscarConteudosHandler(IConteudoQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<IReadOnlyList<ConteudoResumoDto>> Handle(BuscarConteudosQuery query, CancellationToken ct)
    {
        // Use listar with pagination to get all non-filho content, then filter client-side
        // Limited to 10 results for autocomplete
        var paginacao = new PaginacaoParams(1, 50);
        var resultado = await _queryService.ListarAsync(query.UsuarioId, paginacao, ct).ConfigureAwait(false);

        return resultado.Items
            .Where(c => c.Id != query.ExcluirId &&
                        c.Titulo.Contains(query.Prefixo, StringComparison.OrdinalIgnoreCase))
            .Take(10)
            .Select(c => new ConteudoResumoDto(c.Id, c.Titulo, c.Formato.ToString(), c.Papel.ToString(), c.CriadoEm, c.Classificacao, c.Subtipo, null))
            .ToList()
            .AsReadOnly();
    }
}
