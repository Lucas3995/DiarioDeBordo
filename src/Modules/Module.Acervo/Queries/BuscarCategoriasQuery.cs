using System.Diagnostics.CodeAnalysis;
using DiarioDeBordo.Core.Repositorios;
using DiarioDeBordo.Module.Acervo.DTOs;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Queries;

/// <summary>Busca categorias para autocomplete (pelo prefixo do nome).</summary>
public sealed record BuscarCategoriasQuery(
    Guid UsuarioId,
    string Prefixo) : IRequest<IReadOnlyList<CategoriaDto>>;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes",
    Justification = "Instantiated by MediatR via DI container at runtime.")]
internal sealed class BuscarCategoriasHandler : IRequestHandler<BuscarCategoriasQuery, IReadOnlyList<CategoriaDto>>
{
    private readonly ICategoriaRepository _categoriaRepo;

    public BuscarCategoriasHandler(ICategoriaRepository categoriaRepo)
    {
        _categoriaRepo = categoriaRepo;
    }

    public async Task<IReadOnlyList<CategoriaDto>> Handle(BuscarCategoriasQuery query, CancellationToken ct)
    {
        var categorias = await _categoriaRepo
            .ListarComAutocompletarAsync(query.UsuarioId, query.Prefixo, ct)
            .ConfigureAwait(false);

        return categorias
            .Select(c => new CategoriaDto(c.Id, c.Nome, false))
            .ToList()
            .AsReadOnly();
    }
}
