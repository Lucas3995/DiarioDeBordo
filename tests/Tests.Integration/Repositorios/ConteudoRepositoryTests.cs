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

    [Fact]
    public async Task AtualizarAsync_PersisteTituloAlterado()
    {
        var repo = new ConteudoRepository(Context);
        var usuarioId = Guid.NewGuid();

        var conteudo = Conteudo.Criar(usuarioId, "Título Original");
        await repo.AdicionarAsync(conteudo, CancellationToken.None);

        conteudo.Titulo = "Título Alterado";
        await repo.AtualizarAsync(conteudo, CancellationToken.None);

        using var ctx2 = CriarNovoContexto();
        var repo2 = new ConteudoRepository(ctx2);
        var atualizado = await repo2.ObterPorIdAsync(conteudo.Id, usuarioId, CancellationToken.None);

        Assert.NotNull(atualizado);
        Assert.Equal("Título Alterado", atualizado.Titulo);
    }

    [Fact]
    public async Task BuscarPorIdentificadorFonteAsync_EncontradoPorPlataformaEIdentificador()
    {
        var repo = new ConteudoRepository(Context);
        var usuarioId = Guid.NewGuid();

        var conteudo = Conteudo.Criar(usuarioId, "Vídeo YouTube");
        conteudo.AdicionarFonte(TipoFonte.Url, "dQw4w9WgXcQ", prioridade: 1, plataforma: "youtube");
        await repo.AdicionarAsync(conteudo, CancellationToken.None);

        using var ctx2 = CriarNovoContexto();
        var repo2 = new ConteudoRepository(ctx2);
        var encontrado = await repo2.BuscarPorIdentificadorFonteAsync(
            usuarioId, "youtube", "dQw4w9WgXcQ", CancellationToken.None);

        Assert.NotNull(encontrado);
        Assert.Equal("Vídeo YouTube", encontrado.Titulo);
    }

    [Fact]
    public async Task BuscarPorIdentificadorFonteAsync_UsuarioErrado_RetornaNull()
    {
        var repo = new ConteudoRepository(Context);
        var usuarioId = Guid.NewGuid();

        var conteudo = Conteudo.Criar(usuarioId, "Vídeo Privado");
        conteudo.AdicionarFonte(TipoFonte.Url, "abc123", prioridade: 1, plataforma: "youtube");
        await repo.AdicionarAsync(conteudo, CancellationToken.None);

        using var ctx2 = CriarNovoContexto();
        var repo2 = new ConteudoRepository(ctx2);
        var resultado = await repo2.BuscarPorIdentificadorFonteAsync(
            Guid.NewGuid(), "youtube", "abc123", CancellationToken.None);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task AtualizarCategoriasAsync_SubstituiCategoriaExistentePorNova()
    {
        var repo = new ConteudoRepository(Context);
        var usuarioId = Guid.NewGuid();

        var conteudo = Conteudo.Criar(usuarioId, "Duna");
        await repo.AdicionarAsync(conteudo, CancellationToken.None);

        var cat1 = Categoria.Criar(usuarioId, "Ficção");
        var cat2 = Categoria.Criar(usuarioId, "Aventura");
        await Context.Categorias.AddRangeAsync(cat1, cat2);
        await Context.SaveChangesAsync();

        // Associate cat1 first
        await repo.AtualizarCategoriasAsync(conteudo.Id, usuarioId, [cat1.Id], CancellationToken.None);

        // Replace with cat2
        await repo.AtualizarCategoriasAsync(conteudo.Id, usuarioId, [cat2.Id], CancellationToken.None);

        using var ctx2 = CriarNovoContexto();
        var categorias = ctx2.ConteudoCategorias
            .Where(cc => cc.ConteudoId == conteudo.Id)
            .ToList();

        Assert.Single(categorias);
        Assert.Equal(cat2.Id, categorias[0].CategoriaId);
    }

    [Fact]
    public async Task RemoverTodasCategoriasAsync_RemoveTodasAssociacoes()
    {
        var repo = new ConteudoRepository(Context);
        var usuarioId = Guid.NewGuid();

        var conteudo = Conteudo.Criar(usuarioId, "Fundação");
        await repo.AdicionarAsync(conteudo, CancellationToken.None);

        var cat = Categoria.Criar(usuarioId, "Clássico");
        await Context.Categorias.AddAsync(cat);
        await Context.ConteudoCategorias.AddAsync(new ConteudoCategoria
        {
            ConteudoId = conteudo.Id,
            CategoriaId = cat.Id,
            AssociadaEm = DateTimeOffset.UtcNow,
        });
        await Context.SaveChangesAsync();

        await repo.RemoverTodasCategoriasAsync(conteudo.Id, CancellationToken.None);

        using var ctx2 = CriarNovoContexto();
        var categorias = ctx2.ConteudoCategorias
            .Where(cc => cc.ConteudoId == conteudo.Id)
            .ToList();

        Assert.Empty(categorias);
    }

    [Fact]
    public async Task ListarFilhosAsync_RetornaIdsFilhosViaContem()
    {
        var contemTipoId = Guid.Parse("10000000-0000-0000-0000-000000000009");
        var repo = new ConteudoRepository(Context);
        var usuarioId = Guid.NewGuid();

        var pai = Conteudo.Criar(usuarioId, "Breaking Bad");
        var filho = Conteudo.CriarComoFilho(usuarioId, "S01E01");
        await Context.Conteudos.AddRangeAsync(pai, filho);

        var fwdId = Guid.NewGuid();
        var invId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        await Context.Relacoes.AddRangeAsync(
            new Relacao { Id = fwdId, ConteudoOrigemId = pai.Id, ConteudoDestinoId = filho.Id, TipoRelacaoId = contemTipoId, IsInversa = false, ParId = invId, UsuarioId = usuarioId, CriadoEm = now },
            new Relacao { Id = invId, ConteudoOrigemId = filho.Id, ConteudoDestinoId = pai.Id, TipoRelacaoId = contemTipoId, IsInversa = true, ParId = fwdId, UsuarioId = usuarioId, CriadoEm = now }
        );
        await Context.SaveChangesAsync();

        var filhos = await repo.ListarFilhosAsync(pai.Id, usuarioId, contemTipoId, CancellationToken.None);

        Assert.Single(filhos);
        Assert.Equal(filho.Id, filhos[0]);
    }
}
