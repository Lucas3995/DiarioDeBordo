using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Infrastructure.Repositorios;
using DiarioDeBordo.Tests.Integration.Infrastructure;
using Xunit;

namespace DiarioDeBordo.Tests.Integration.Repositorios;

/// <summary>
/// Testes de integração do TipoRelacaoRepository.
/// Verifica deduplicação case-insensitive, tipos de sistema, e autocomplete.
/// </summary>
public class TipoRelacaoRepositoryTests : PostgresTestBase
{
    [Fact]
    public async Task ObterOuCriarAsync_NovaTipo_CriaERetorna()
    {
        var repo = new TipoRelacaoRepository(Context);
        var usuarioId = Guid.NewGuid();

        var tipo = await repo.ObterOuCriarAsync(usuarioId, "Inspiração", "Inspirou", CancellationToken.None);

        Assert.NotNull(tipo);
        Assert.Equal("Inspiração", tipo.Nome);
        Assert.Equal("Inspirou", tipo.NomeInverso);
        Assert.Equal("inspiração", tipo.NomeNormalizado);
        Assert.NotEqual(Guid.Empty, tipo.Id);
        Assert.False(tipo.IsSistema);
    }

    [Fact]
    public async Task ObterOuCriarAsync_TipoDuplicadoCaseInsensitive_RetornaExistente()
    {
        var repo = new TipoRelacaoRepository(Context);
        var usuarioId = Guid.NewGuid();

        var tipo1 = await repo.ObterOuCriarAsync(usuarioId, "Inspiração", "Inspirou", CancellationToken.None);
        var tipo2 = await repo.ObterOuCriarAsync(usuarioId, "inspiração", "Inspirou", CancellationToken.None);
        var tipo3 = await repo.ObterOuCriarAsync(usuarioId, "INSPIRAÇÃO", "Inspirou", CancellationToken.None);

        Assert.Equal(tipo1.Id, tipo2.Id);
        Assert.Equal(tipo1.Id, tipo3.Id);
    }

    [Fact]
    public async Task ObterOuCriarAsync_TipoSistema_RetornaTipoSistema()
    {
        var repo = new TipoRelacaoRepository(Context);
        var usuarioId = Guid.NewGuid();

        // "Sequência" is a pre-seeded system type (D-13)
        var tipo = await repo.ObterOuCriarAsync(usuarioId, "Sequência", "Continuação de", CancellationToken.None);

        Assert.True(tipo.IsSistema);
        Assert.Equal(Guid.Parse("10000000-0000-0000-0000-000000000001"), tipo.Id);
    }

    [Fact]
    public async Task ListarComAutocompletarAsync_IncluiTiposSistemaEUsuario()
    {
        var repo = new TipoRelacaoRepository(Context);
        var usuarioId = Guid.NewGuid();

        // Create user type starting with "seq"
        await repo.ObterOuCriarAsync(usuarioId, "Sequencial", "Antecede", CancellationToken.None);

        using var ctx2 = CriarNovoContexto();
        var repo2 = new TipoRelacaoRepository(ctx2);
        var resultados = await repo2.ListarComAutocompletarAsync(usuarioId, "seq", CancellationToken.None);

        // Should include both: system "Sequência" and user "Sequencial"
        Assert.True(resultados.Count >= 2);
        Assert.Contains(resultados, t => t.Nome == "Sequência" && t.IsSistema);
        Assert.Contains(resultados, t => t.Nome == "Sequencial" && !t.IsSistema);
    }

    [Fact]
    public async Task ObterPorIdAsync_TipoExistente_Retorna()
    {
        var repo = new TipoRelacaoRepository(Context);
        var usuarioId = Guid.NewGuid();

        // System type is visible to any user
        var tipo = await repo.ObterPorIdAsync(
            Guid.Parse("10000000-0000-0000-0000-000000000009"), // "Contém"
            usuarioId,
            CancellationToken.None);

        Assert.NotNull(tipo);
        Assert.Equal("Contém", tipo!.Nome);
        Assert.True(tipo.IsSistema);
    }

    [Fact]
    public async Task SeedData_NoveTiposSistemaPresentes()
    {
        var tiposSistema = Context.TipoRelacoes.Where(t => t.IsSistema).ToList();

        Assert.Equal(9, tiposSistema.Count);
        Assert.Contains(tiposSistema, t => t.Nome == "Contém" && t.NomeInverso == "Parte de");
        Assert.Contains(tiposSistema, t => t.Nome == "Sequência");
    }
}
