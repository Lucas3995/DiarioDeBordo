using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Core.Enums;
using DiarioDeBordo.Infrastructure.Repositorios;
using DiarioDeBordo.Tests.Integration.Infrastructure;
using Xunit;

namespace DiarioDeBordo.Tests.Integration.Repositorios;

/// <summary>
/// Testes de integração do ColetaneaRepository contra PostgreSQL real (Testcontainers).
/// Verificam: cycle detection via BFS, item management, security via usuarioId.
/// </summary>
public class ColetaneaRepositoryTests : PostgresTestBase
{
    private static Conteudo CriarColetanea(Guid usuarioId, string titulo, TipoColetanea tipo = TipoColetanea.Miscelanea)
    {
        return Conteudo.Criar(usuarioId, titulo, PapelConteudo.Coletanea, tipo);
    }

    [Fact]
    public async Task Given_CadeiaABC_When_ObterDescendentesA_Then_RetornaBEC()
    {
        // Arrange: Create collections A -> B -> C chain
        var repo = new ColetaneaRepository(Context);
        var usuarioId = Guid.NewGuid();

        var colA = CriarColetanea(usuarioId, "Coletânea A");
        var colB = CriarColetanea(usuarioId, "Coletânea B");
        var colC = CriarColetanea(usuarioId, "Coletânea C");

        await Context.Conteudos.AddRangeAsync(colA, colB, colC);
        await Context.SaveChangesAsync();

        // Add B to A, C to B
        var now = DateTimeOffset.UtcNow;
        await repo.AdicionarItemAsync(new ConteudoColetanea
        {
            ColetaneaId = colA.Id,
            ConteudoId = colB.Id,
            Posicao = 1,
            AdicionadoEm = now
        }, CancellationToken.None);

        await repo.AdicionarItemAsync(new ConteudoColetanea
        {
            ColetaneaId = colB.Id,
            ConteudoId = colC.Id,
            Posicao = 1,
            AdicionadoEm = now
        }, CancellationToken.None);

        // Act
        var descendentes = await repo.ObterDescendentesAsync(colA.Id, usuarioId, CancellationToken.None);

        // Assert: descendants of A = {B, C}
        Assert.Equal(2, descendentes.Count);
        Assert.Contains(colB.Id, descendentes);
        Assert.Contains(colC.Id, descendentes);
    }

    [Fact]
    public async Task Given_CicloABC_When_ObterDescendentesC_Then_RetornaConjuntoComA()
    {
        // Arrange: Create A -> B -> C cycle (add A to C to complete cycle)
        var repo = new ColetaneaRepository(Context);
        var usuarioId = Guid.NewGuid();

        var colA = CriarColetanea(usuarioId, "Coletânea A");
        var colB = CriarColetanea(usuarioId, "Coletânea B");
        var colC = CriarColetanea(usuarioId, "Coletânea C");

        await Context.Conteudos.AddRangeAsync(colA, colB, colC);
        await Context.SaveChangesAsync();

        var now = DateTimeOffset.UtcNow;
        await repo.AdicionarItemAsync(new ConteudoColetanea
        {
            ColetaneaId = colA.Id,
            ConteudoId = colB.Id,
            Posicao = 1,
            AdicionadoEm = now
        }, CancellationToken.None);

        await repo.AdicionarItemAsync(new ConteudoColetanea
        {
            ColetaneaId = colB.Id,
            ConteudoId = colC.Id,
            Posicao = 1,
            AdicionadoEm = now
        }, CancellationToken.None);

        // Complete cycle: add A to C
        await repo.AdicionarItemAsync(new ConteudoColetanea
        {
            ColetaneaId = colC.Id,
            ConteudoId = colA.Id,
            Posicao = 1,
            AdicionadoEm = now
        }, CancellationToken.None);

        // Act: check descendants of C (should detect cycle)
        var descendentes = await repo.ObterDescendentesAsync(colC.Id, usuarioId, CancellationToken.None);

        // Assert: descendants of C includes A (cycle detected)
        Assert.Contains(colA.Id, descendentes);
        // Also should include B since A contains B
        Assert.Contains(colB.Id, descendentes);
    }

    [Fact]
    public async Task Given_ColetaneaVazia_When_ObterDescendentes_Then_RetornaListaVazia()
    {
        var repo = new ColetaneaRepository(Context);
        var usuarioId = Guid.NewGuid();

        var col = CriarColetanea(usuarioId, "Coletânea Vazia");
        await Context.Conteudos.AddAsync(col);
        await Context.SaveChangesAsync();

        var descendentes = await repo.ObterDescendentesAsync(col.Id, usuarioId, CancellationToken.None);

        Assert.Empty(descendentes);
    }

    [Fact]
    public async Task Given_ColetaneaCom3Itens_When_ObterProximaPosicao_Then_Retorna4()
    {
        var repo = new ColetaneaRepository(Context);
        var usuarioId = Guid.NewGuid();

        var col = CriarColetanea(usuarioId, "Coletânea com Itens");
        var item1 = Conteudo.Criar(usuarioId, "Item 1");
        var item2 = Conteudo.Criar(usuarioId, "Item 2");
        var item3 = Conteudo.Criar(usuarioId, "Item 3");

        await Context.Conteudos.AddRangeAsync(col, item1, item2, item3);
        await Context.SaveChangesAsync();

        var now = DateTimeOffset.UtcNow;
        await repo.AdicionarItemAsync(new ConteudoColetanea
        {
            ColetaneaId = col.Id,
            ConteudoId = item1.Id,
            Posicao = 1,
            AdicionadoEm = now
        }, CancellationToken.None);
        await repo.AdicionarItemAsync(new ConteudoColetanea
        {
            ColetaneaId = col.Id,
            ConteudoId = item2.Id,
            Posicao = 2,
            AdicionadoEm = now
        }, CancellationToken.None);
        await repo.AdicionarItemAsync(new ConteudoColetanea
        {
            ColetaneaId = col.Id,
            ConteudoId = item3.Id,
            Posicao = 3,
            AdicionadoEm = now
        }, CancellationToken.None);

        var proximaPosicao = await repo.ObterProximaPosicaoAsync(col.Id, CancellationToken.None);

        Assert.Equal(4, proximaPosicao);
    }

    [Fact]
    public async Task Given_ColetaneaVazia_When_ObterProximaPosicao_Then_Retorna1()
    {
        var repo = new ColetaneaRepository(Context);
        var usuarioId = Guid.NewGuid();

        var col = CriarColetanea(usuarioId, "Coletânea Vazia");
        await Context.Conteudos.AddAsync(col);
        await Context.SaveChangesAsync();

        var proximaPosicao = await repo.ObterProximaPosicaoAsync(col.Id, CancellationToken.None);

        Assert.Equal(1, proximaPosicao);
    }

    [Fact]
    public async Task Given_ItemAdicionado_When_ObterItem_Then_RetornaTodosOsCampos()
    {
        var repo = new ColetaneaRepository(Context);
        var usuarioId = Guid.NewGuid();

        var col = CriarColetanea(usuarioId, "Coletânea");
        var item = Conteudo.Criar(usuarioId, "Item com Anotação");

        await Context.Conteudos.AddRangeAsync(col, item);
        await Context.SaveChangesAsync();

        var anotacao = "Esta é uma anotação contextual de teste.";
        var adicionadoEm = DateTimeOffset.UtcNow;
        await repo.AdicionarItemAsync(new ConteudoColetanea
        {
            ColetaneaId = col.Id,
            ConteudoId = item.Id,
            Posicao = 42,
            AnotacaoContextual = anotacao,
            AdicionadoEm = adicionadoEm
        }, CancellationToken.None);

        using var ctx2 = CriarNovoContexto();
        var repo2 = new ColetaneaRepository(ctx2);
        var recuperado = await repo2.ObterItemAsync(col.Id, item.Id, usuarioId, CancellationToken.None);

        Assert.NotNull(recuperado);
        Assert.Equal(col.Id, recuperado.ColetaneaId);
        Assert.Equal(item.Id, recuperado.ConteudoId);
        Assert.Equal(42, recuperado.Posicao);
        Assert.Equal(anotacao, recuperado.AnotacaoContextual);
    }

    [Fact]
    public async Task Given_ItemNaColetanea_When_RemoverItem_Then_AssociacaoRemovidaConteudoPreservado()
    {
        var repo = new ColetaneaRepository(Context);
        var usuarioId = Guid.NewGuid();

        var col = CriarColetanea(usuarioId, "Coletânea");
        var item = Conteudo.Criar(usuarioId, "Item para Remover");

        await Context.Conteudos.AddRangeAsync(col, item);
        await Context.SaveChangesAsync();

        await repo.AdicionarItemAsync(new ConteudoColetanea
        {
            ColetaneaId = col.Id,
            ConteudoId = item.Id,
            Posicao = 1,
            AdicionadoEm = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        // Verify item exists
        var existe = await repo.ItemExisteNaColetaneaAsync(col.Id, item.Id, CancellationToken.None);
        Assert.True(existe);

        // Remove association
        await repo.RemoverItemAsync(col.Id, item.Id, usuarioId, CancellationToken.None);

        // Verify association gone
        using var ctx2 = CriarNovoContexto();
        var repo2 = new ColetaneaRepository(ctx2);
        var existeDepois = await repo2.ItemExisteNaColetaneaAsync(col.Id, item.Id, CancellationToken.None);
        Assert.False(existeDepois);

        // Verify content still exists
        var itemAinda = await ctx2.Conteudos.FindAsync(item.Id);
        Assert.NotNull(itemAinda);
        Assert.Equal("Item para Remover", itemAinda.Titulo);
    }

    [Fact]
    public async Task Given_ItemNaColetanea_When_ItemExisteNaColetanea_Then_RetornaTrue()
    {
        var repo = new ColetaneaRepository(Context);
        var usuarioId = Guid.NewGuid();

        var col = CriarColetanea(usuarioId, "Coletânea");
        var item = Conteudo.Criar(usuarioId, "Item");

        await Context.Conteudos.AddRangeAsync(col, item);
        await Context.SaveChangesAsync();

        await repo.AdicionarItemAsync(new ConteudoColetanea
        {
            ColetaneaId = col.Id,
            ConteudoId = item.Id,
            Posicao = 1,
            AdicionadoEm = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        var existe = await repo.ItemExisteNaColetaneaAsync(col.Id, item.Id, CancellationToken.None);

        Assert.True(existe);
    }

    [Fact]
    public async Task Given_ItemForaDaColetanea_When_ItemExisteNaColetanea_Then_RetornaFalse()
    {
        var repo = new ColetaneaRepository(Context);
        var usuarioId = Guid.NewGuid();

        var col = CriarColetanea(usuarioId, "Coletânea");
        var item = Conteudo.Criar(usuarioId, "Item não adicionado");

        await Context.Conteudos.AddRangeAsync(col, item);
        await Context.SaveChangesAsync();

        // Don't add item to collection
        var existe = await repo.ItemExisteNaColetaneaAsync(col.Id, item.Id, CancellationToken.None);

        Assert.False(existe);
    }

    [Fact]
    public async Task Given_ColetaneaOutroUsuario_When_ObterDescendentes_Then_RetornaVazio()
    {
        // SECURITY: BFS should not traverse collections owned by other users
        var repo = new ColetaneaRepository(Context);
        var usuario1 = Guid.NewGuid();
        var usuario2 = Guid.NewGuid();

        var colUsuario1 = CriarColetanea(usuario1, "Coletânea Usuário 1");
        var colUsuario2 = CriarColetanea(usuario2, "Coletânea Usuário 2");

        await Context.Conteudos.AddRangeAsync(colUsuario1, colUsuario2);
        await Context.SaveChangesAsync();

        // User1's collection contains User2's collection (unusual but possible via some future feature)
        await repo.AdicionarItemAsync(new ConteudoColetanea
        {
            ColetaneaId = colUsuario1.Id,
            ConteudoId = colUsuario2.Id,
            Posicao = 1,
            AdicionadoEm = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        // User2 checks descendants of User1's collection — should return empty (SEG-02)
        var descendentes = await repo.ObterDescendentesAsync(colUsuario1.Id, usuario2, CancellationToken.None);

        // User2's collection is owned by user2, not user2 querying user1's subtree
        // The BFS filters by usuarioId, so it won't find user2's collection as a child
        Assert.Empty(descendentes);
    }
}
