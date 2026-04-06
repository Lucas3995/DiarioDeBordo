using System.Diagnostics.CodeAnalysis;
using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Core.Repositorios;
using DiarioDeBordo.Module.Acervo.DTOs;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Commands;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes",
    Justification = "Instantiated by MediatR via DI container at runtime.")]
internal sealed class ObterOuCriarCategoriaHandler : IRequestHandler<ObterOuCriarCategoriaCommand, Resultado<CategoriaDto>>
{
    private readonly ICategoriaRepository _repo;

    public ObterOuCriarCategoriaHandler(ICategoriaRepository repo)
    {
        _repo = repo;
    }

    public async Task<Resultado<CategoriaDto>> Handle(ObterOuCriarCategoriaCommand cmd, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(cmd.Nome))
            return Resultado<CategoriaDto>.Failure(Erros.TituloObrigatorio);

        var categoria = await _repo.ObterOuCriarAsync(cmd.UsuarioId, cmd.Nome.Trim(), ct).ConfigureAwait(false);

        // Categorias criadas inline pelo usuário são sempre manuais (IsAutomatica = false)
        return Resultado<CategoriaDto>.Success(new CategoriaDto(categoria.Id, categoria.Nome, IsAutomatica: false));
    }
}
