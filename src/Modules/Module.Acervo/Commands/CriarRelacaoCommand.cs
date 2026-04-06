using DiarioDeBordo.Core.Primitivos;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Commands;

/// <summary>
/// Cria uma relação bidirecional entre dois conteúdos com um tipo de relação.
/// Cria o TipoRelacao se ainda não existir (ObterOuCriarAsync).
/// </summary>
public sealed record CriarRelacaoCommand(
    Guid UsuarioId,
    Guid ConteudoOrigemId,
    Guid ConteudoDestinoId,
    string NomeTipoRelacao,
    string NomeInverso) : IRequest<Resultado<Guid>>;
