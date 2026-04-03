using DiarioDeBordo.Infrastructure.Repositorios;
using DiarioDeBordo.Tests.Integration.Infrastructure;
using Xunit;

namespace DiarioDeBordo.Tests.Integration.Repositorios;

/// <summary>
/// Testes de integração do CategoriaRepository — foco em I-12 (unicidade case-insensitive).
/// </summary>
public class CategoriaRepositoryTests : PostgresTestBase
{
    // ---- I-12: Categoria case-insensitive única por usuário ----

    [Fact]
    public async Task ObterOuCriar_PrimeiraVez_CriaNova()
    {
        var repo = new CategoriaRepository(Context);
        var usuarioId = Guid.NewGuid();

        var categoria = await repo.ObterOuCriarAsync(usuarioId, "Romance", CancellationToken.None);

        Assert.NotNull(categoria);
        Assert.Equal("Romance", categoria.Nome);
        Assert.Equal("romance", categoria.NomeNormalizado);
        Assert.NotEqual(Guid.Empty, categoria.Id);
    }

    [Fact]
    public async Task ObterOuCriar_NomeDuplicadoMesmoCase_RetornaExistente()
    {
        var repo = new CategoriaRepository(Context);
        var usuarioId = Guid.NewGuid();

        var categoria1 = await repo.ObterOuCriarAsync(usuarioId, "Romance", CancellationToken.None);
        var categoria2 = await repo.ObterOuCriarAsync(usuarioId, "Romance", CancellationToken.None);

        Assert.Equal(categoria1.Id, categoria2.Id); // Same record returned
    }

    [Fact]
    public async Task ObterOuCriar_NomeDuplicadoCaseInsensitive_RetornaExistente()
    {
        // I-12: "Romance" and "romance" are the same category
        var repo = new CategoriaRepository(Context);
        var usuarioId = Guid.NewGuid();

        var categoriaRomance = await repo.ObterOuCriarAsync(usuarioId, "Romance", CancellationToken.None);
        var categoriaRomanceMin = await repo.ObterOuCriarAsync(usuarioId, "romance", CancellationToken.None);
        var categoriaROMANCE = await repo.ObterOuCriarAsync(usuarioId, "ROMANCE", CancellationToken.None);

        Assert.Equal(categoriaRomance.Id, categoriaRomanceMin.Id);
        Assert.Equal(categoriaRomance.Id, categoriaROMANCE.Id);
    }

    [Fact]
    public async Task ObterOuCriar_UsuariosDistintos_CategoriasIndependentes()
    {
        var repo = new CategoriaRepository(Context);
        var usuario1 = Guid.NewGuid();
        var usuario2 = Guid.NewGuid();

        var cat1 = await repo.ObterOuCriarAsync(usuario1, "Romance", CancellationToken.None);
        var cat2 = await repo.ObterOuCriarAsync(usuario2, "Romance", CancellationToken.None);

        Assert.NotEqual(cat1.Id, cat2.Id); // Different users have separate categories
    }

    [Fact]
    public async Task ListarComAutocompletar_RetornaMatchesDePrefixo()
    {
        var repo = new CategoriaRepository(Context);
        var usuarioId = Guid.NewGuid();

        await repo.ObterOuCriarAsync(usuarioId, "Romance", CancellationToken.None);
        await repo.ObterOuCriarAsync(usuarioId, "Romantismo", CancellationToken.None);
        await repo.ObterOuCriarAsync(usuarioId, "Ficção Científica", CancellationToken.None);

        using var ctx2 = CriarNovoContexto();
        var repo2 = new CategoriaRepository(ctx2);
        var resultados = await repo2.ListarComAutocompletarAsync(usuarioId, "rom", CancellationToken.None);

        Assert.Equal(2, resultados.Count);
        Assert.All(resultados, c => Assert.StartsWith("rom", c.NomeNormalizado));
    }
}
