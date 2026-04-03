using System.Diagnostics.CodeAnalysis;
using DiarioDeBordo.Core.Consultas;
using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Module.Acervo.DTOs;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Queries;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes",
    Justification = "Instantiated by MediatR via DI container at runtime.")]
internal sealed class ObterConteudoHandler
    : IRequestHandler<ObterConteudoQuery, Resultado<ConteudoDetalheDto>>
{
    private readonly IConteudoQueryService _queryService;

    public ObterConteudoHandler(IConteudoQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<Resultado<ConteudoDetalheDto>> Handle(
        ObterConteudoQuery query, CancellationToken ct)
    {
        var data = await _queryService
            .ObterAsync(query.Id, query.UsuarioId, ct)
            .ConfigureAwait(false);

        if (data is null)
            return Resultado<ConteudoDetalheDto>.Failure(Erros.NaoEncontrado);

        return Resultado<ConteudoDetalheDto>.Success(
            new ConteudoDetalheDto(
                data.Id,
                data.Titulo,
                data.Descricao,
                data.Anotacoes,
                data.Nota,
                data.Formato.ToString(),
                data.Papel.ToString(),
                data.CriadoEm));
    }
}
