using DiarioDeBordo.Core.Primitivos;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Commands;

/// <summary>
/// Cria um novo conteúdo com título (obrigatório).
/// Disclosure progressivo: apenas título é obrigatório na criação (Pesquisa 4.2 — Shneiderman 1996).
/// Descricao e Anotacoes são opcionais e salvos junto na criação quando preenchidos.
/// </summary>
public sealed record CriarConteudoCommand(
    Guid UsuarioId,
    string Titulo,
    string? Descricao = null,
    string? Anotacoes = null) : IRequest<Resultado<Guid>>;
