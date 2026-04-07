using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Module.Acervo.DTOs;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Queries;

public sealed record VerificarDuplicataQuery(
    Guid UsuarioId,
    string Titulo,
    IReadOnlyList<string>? FonteUrls = null) : IRequest<Resultado<DuplicataDto?>>;
