using DiarioDeBordo.Core.Primitivos;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Commands;

/// <summary>
/// Cria um novo conteúdo com título (obrigatório).
/// Disclosure progressivo: apenas título é obrigatório na criação (Pesquisa 4.2 — Shneiderman 1996).
/// Demais campos são enriquecidos posteriormente via comandos específicos.
/// </summary>
public sealed record CriarConteudoCommand(
    Guid UsuarioId,
    string Titulo) : IRequest<Resultado<Guid>>;
