using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Module.Acervo.DTOs;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Commands;

/// <summary>
/// Retorna uma categoria existente (case-insensitive) ou cria uma nova.
/// Usado para criação inline de categorias no formulário de detalhe (D-10).
/// </summary>
public sealed record ObterOuCriarCategoriaCommand(
    Guid UsuarioId,
    string Nome) : IRequest<Resultado<CategoriaDto>>;
