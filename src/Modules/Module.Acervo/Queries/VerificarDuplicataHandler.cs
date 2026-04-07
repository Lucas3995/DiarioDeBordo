using System.Diagnostics.CodeAnalysis;
using DiarioDeBordo.Core.Consultas;
using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Module.Acervo.DTOs;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Queries;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes",
    Justification = "Instantiated by MediatR via DI container at runtime.")]
internal sealed class VerificarDuplicataHandler
    : IRequestHandler<VerificarDuplicataQuery, Resultado<DuplicataDto?>>
{
    private readonly IDeduplicacaoService _deduplicacaoService;

    public VerificarDuplicataHandler(IDeduplicacaoService deduplicacaoService)
    {
        _deduplicacaoService = deduplicacaoService;
    }

    public async Task<Resultado<DuplicataDto?>> Handle(
        VerificarDuplicataQuery query, CancellationToken ct)
    {
        var data = await _deduplicacaoService
            .VerificarAsync(query.UsuarioId, query.Titulo, query.FonteUrls, ct)
            .ConfigureAwait(false);

        if (data is null)
            return Resultado<DuplicataDto?>.Success(null);

        var dto = new DuplicataDto(
            data.ConteudoId,
            data.Titulo,
            data.CriadoEm,
            data.Nivel.ToString());

        return Resultado<DuplicataDto?>.Success(dto);
    }
}
