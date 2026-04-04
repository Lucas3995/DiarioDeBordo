using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Infrastructure.Persistencia;
using DiarioDeBordo.Infrastructure.Repositorios;
using DiarioDeBordo.Tests.Integration.Infrastructure;
using Xunit;

namespace DiarioDeBordo.Tests.Integration.Repositorios;

/// <summary>
/// Testes de integração do RelacaoRepository.
/// Verifica criação atômica do par bidirecional, remoção e segurança por usuário (SEG-02).
/// </summary>
public class RelacaoRepositoryTests : PostgresTestBase
{
    private static readonly Guid _contemTipoId = Guid.Parse("10000000-0000-0000-0000-000000000001");

    private static async Task<Conteudo> CriarConteudoAsync(DiarioDeBordoDbContext ctx, Guid usuarioId, string titulo)
    {
        var c = Conteudo.Criar(usuarioId, titulo);
        await ctx.Conteudos.AddAsync(c);
        await ctx.SaveChangesAsync();
        return c;
    }

    private static (Relacao forward, Relacao inverse) BuildPar(Guid origemId, Guid destinoId, Guid usuarioId)
    {
        var forwardId = Guid.NewGuid();
        var inverseId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        var forward = new Relacao
        {
            Id = forwardId,
            ConteudoOrigemId = origemId,
            ConteudoDestinoId = destinoId,
            TipoRelacaoId = _contemTipoId,
            IsInversa = false,
            ParId = inverseId,
            UsuarioId = usuarioId,
            CriadoEm = now,
        };

        var inverse = new Relacao
        {
            Id = inverseId,
            ConteudoOrigemId = destinoId,
            ConteudoDestinoId = origemId,
            TipoRelacaoId = _contemTipoId,
            IsInversa = true,
            ParId = forwardId,
            UsuarioId = usuarioId,
            CriadoEm = now,
        };

        return (forward, inverse);
    }

    [Fact]
    public async Task AdicionarParAsync_CriaAmbasLinhas()
    {
        var usuarioId = Guid.NewGuid();
        var conteudo1 = await CriarConteudoAsync(Context, usuarioId, "Conteúdo A");
        var conteudo2 = await CriarConteudoAsync(Context, usuarioId, "Conteúdo B");

        var repo = new RelacaoRepository(Context);
        var (forward, inverse) = BuildPar(conteudo1.Id, conteudo2.Id, usuarioId);
        await repo.AdicionarParAsync(forward, inverse, CancellationToken.None);

        using var ctx2 = CriarNovoContexto();
        var rows = ctx2.Relacoes.Where(r => r.UsuarioId == usuarioId).ToList();
        Assert.Equal(2, rows.Count);
        Assert.Contains(rows, r => r.IsInversa == false);
        Assert.Contains(rows, r => r.IsInversa == true);
    }

    [Fact]
    public async Task RemoverParAsync_RemoveAmbosDosLados()
    {
        var usuarioId = Guid.NewGuid();
        var conteudo1 = await CriarConteudoAsync(Context, usuarioId, "Conteúdo A");
        var conteudo2 = await CriarConteudoAsync(Context, usuarioId, "Conteúdo B");

        var repo = new RelacaoRepository(Context);
        var (forward, inverse) = BuildPar(conteudo1.Id, conteudo2.Id, usuarioId);
        await repo.AdicionarParAsync(forward, inverse, CancellationToken.None);

        using var ctx2 = CriarNovoContexto();
        var repo2 = new RelacaoRepository(ctx2);
        await repo2.RemoverParAsync(forward.Id, usuarioId, CancellationToken.None);

        using var ctx3 = CriarNovoContexto();
        var rows = ctx3.Relacoes.Where(r => r.UsuarioId == usuarioId).ToList();
        Assert.Empty(rows);
    }

    [Fact]
    public async Task ExisteAsync_RetornaTrue_QuandoExiste()
    {
        var usuarioId = Guid.NewGuid();
        var conteudo1 = await CriarConteudoAsync(Context, usuarioId, "Conteúdo A");
        var conteudo2 = await CriarConteudoAsync(Context, usuarioId, "Conteúdo B");

        var repo = new RelacaoRepository(Context);
        var (forward, inverse) = BuildPar(conteudo1.Id, conteudo2.Id, usuarioId);
        await repo.AdicionarParAsync(forward, inverse, CancellationToken.None);

        using var ctx2 = CriarNovoContexto();
        var repo2 = new RelacaoRepository(ctx2);
        var existe = await repo2.ExisteAsync(conteudo1.Id, conteudo2.Id, _contemTipoId, usuarioId, CancellationToken.None);

        Assert.True(existe);
    }

    [Fact]
    public async Task ExisteAsync_RetornaFalse_QuandoNaoExiste()
    {
        var usuarioId = Guid.NewGuid();
        var repo = new RelacaoRepository(Context);

        var existe = await repo.ExisteAsync(Guid.NewGuid(), Guid.NewGuid(), _contemTipoId, usuarioId, CancellationToken.None);

        Assert.False(existe);
    }

    [Fact]
    public async Task ListarPorConteudoAsync_FiltraPorUsuarioId()
    {
        var usuario1 = Guid.NewGuid();
        var usuario2 = Guid.NewGuid();
        var c1 = await CriarConteudoAsync(Context, usuario1, "U1 A");
        var c2 = await CriarConteudoAsync(Context, usuario1, "U1 B");
        var c3 = await CriarConteudoAsync(Context, usuario2, "U2 A");
        var c4 = await CriarConteudoAsync(Context, usuario2, "U2 B");

        var repo = new RelacaoRepository(Context);
        var (f1, i1) = BuildPar(c1.Id, c2.Id, usuario1);
        var (f2, i2) = BuildPar(c3.Id, c4.Id, usuario2);
        await repo.AdicionarParAsync(f1, i1, CancellationToken.None);

        using var ctx2 = CriarNovoContexto();
        var repo2 = new RelacaoRepository(ctx2);
        await repo2.AdicionarParAsync(f2, i2, CancellationToken.None);

        using var ctx3 = CriarNovoContexto();
        var repo3 = new RelacaoRepository(ctx3);
        var u1Relacoes = await repo3.ListarPorConteudoAsync(c1.Id, usuario1, CancellationToken.None);

        // SEG-02: only user1's relations for c1 returned
        Assert.All(u1Relacoes, r => Assert.Equal(usuario1, r.UsuarioId));
        Assert.DoesNotContain(u1Relacoes, r => r.UsuarioId == usuario2);
    }
}
