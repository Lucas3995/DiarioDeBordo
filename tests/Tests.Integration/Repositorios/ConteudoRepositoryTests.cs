using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Core.Enums;
using DiarioDeBordo.Infrastructure.Repositorios;
using DiarioDeBordo.Tests.Integration.Infrastructure;
using Xunit;

namespace DiarioDeBordo.Tests.Integration.Repositorios;

/// <summary>
/// Testes de integração do ConteudoRepository contra PostgreSQL real (Testcontainers).
/// Verificam as garantias de segurança: todo acesso inclui usuarioId.
/// </summary>
public class ConteudoRepositoryTests : PostgresTestBase
{
    [Fact]
    public async Task AdicionarAsync_EObterPorId_RetornaConteudo()
    {
        var repo = new ConteudoRepository(Context);
        var usuarioId = Guid.NewGuid();

        var conteudo = Conteudo.Criar(usuarioId, "Dune");
        await repo.AdicionarAsync(conteudo, CancellationToken.None);

        // Use a fresh context to verify actual persistence (not EF change tracker)
        using var ctx2 = CriarNovoContexto();
        var repo2 = new ConteudoRepository(ctx2);
        var recuperado = await repo2.ObterPorIdAsync(conteudo.Id, usuarioId, CancellationToken.None);

        Assert.NotNull(recuperado);
        Assert.Equal("Dune", recuperado.Titulo);
        Assert.Equal(usuarioId, recuperado.UsuarioId);
    }

    [Fact]
    public async Task ObterPorId_UsuarioIdErrado_RetornaNull()
    {
        // SECURITY: users cannot access each other's content
        var repo = new ConteudoRepository(Context);
        var usuarioIdCorreto = Guid.NewGuid();
        var usuarioIdErrado = Guid.NewGuid();

        var conteudo = Conteudo.Criar(usuarioIdCorreto, "Conteúdo Privado");
        await repo.AdicionarAsync(conteudo, CancellationToken.None);

        var resultado = await repo.ObterPorIdAsync(conteudo.Id, usuarioIdErrado, CancellationToken.None);

        Assert.Null(resultado); // Different user cannot see this content
    }

    [Fact]
    public async Task BuscarPorUrlFonte_EncontradaParaUsuarioCorreto()
    {
        var repo = new ConteudoRepository(Context);
        var usuarioId = Guid.NewGuid();

        var conteudo = Conteudo.Criar(usuarioId, "Artigo Científico");
        conteudo.AdicionarFonte(TipoFonte.Url, "https://example.com/artigo", prioridade: 1);
        await repo.AdicionarAsync(conteudo, CancellationToken.None);

        using var ctx2 = CriarNovoContexto();
        var repo2 = new ConteudoRepository(ctx2);
        var encontrado = await repo2.BuscarPorUrlFonteAsync(
            usuarioId, "https://example.com/artigo", CancellationToken.None);

        Assert.NotNull(encontrado);
        Assert.Equal("Artigo Científico", encontrado.Titulo);
    }

    [Fact]
    public async Task BuscarPorUrlFonte_UsuarioErrado_RetornaNull()
    {
        var repo = new ConteudoRepository(Context);
        var usuarioId = Guid.NewGuid();
        var outroUsuario = Guid.NewGuid();

        var conteudo = Conteudo.Criar(usuarioId, "Meu Artigo");
        conteudo.AdicionarFonte(TipoFonte.Url, "https://example.com/meu", prioridade: 1);
        await repo.AdicionarAsync(conteudo, CancellationToken.None);

        using var ctx2 = CriarNovoContexto();
        var repo2 = new ConteudoRepository(ctx2);
        var resultado = await repo2.BuscarPorUrlFonteAsync(
            outroUsuario, "https://example.com/meu", CancellationToken.None);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task RemoverAsync_ConteudoExistente_NaoEncontradoDepois()
    {
        var repo = new ConteudoRepository(Context);
        var usuarioId = Guid.NewGuid();

        var conteudo = Conteudo.Criar(usuarioId, "Para Remover");
        await repo.AdicionarAsync(conteudo, CancellationToken.None);

        await repo.RemoverAsync(conteudo.Id, usuarioId, CancellationToken.None);

        using var ctx2 = CriarNovoContexto();
        var repo2 = new ConteudoRepository(ctx2);
        var resultado = await repo2.ObterPorIdAsync(conteudo.Id, usuarioId, CancellationToken.None);

        Assert.Null(resultado);
    }
}
