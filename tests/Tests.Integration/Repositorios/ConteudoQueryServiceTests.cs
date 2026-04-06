using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Core.Enums;
using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Infrastructure.Consultas;
using DiarioDeBordo.Tests.Integration.Infrastructure;
using Xunit;

namespace DiarioDeBordo.Tests.Integration.Repositorios;

/// <summary>
/// Testes de integração do ConteudoQueryService (CQRS — query side).
/// Cobrem ObterAsync e ListarSessoesAsync contra PostgreSQL real.
/// </summary>
public class ConteudoQueryServiceTests : PostgresTestBase
{
    private static readonly Guid _contemTipoId = Guid.Parse("10000000-0000-0000-0000-000000000009");
    private static readonly Guid _sequenciaTipoId = Guid.Parse("10000000-0000-0000-0000-000000000001");

    private static (Relacao forward, Relacao inverse) BuildPar(
        Guid origemId, Guid destinoId, Guid tipoId, Guid usuarioId)
    {
        var fwdId = Guid.NewGuid();
        var invId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        return (
            new Relacao { Id = fwdId, ConteudoOrigemId = origemId, ConteudoDestinoId = destinoId, TipoRelacaoId = tipoId, IsInversa = false, ParId = invId, UsuarioId = usuarioId, CriadoEm = now },
            new Relacao { Id = invId, ConteudoOrigemId = destinoId, ConteudoDestinoId = origemId, TipoRelacaoId = tipoId, IsInversa = true, ParId = fwdId, UsuarioId = usuarioId, CriadoEm = now }
        );
    }

    [Fact]
    public async Task ObterAsync_ConteudoInexistente_RetornaNull()
    {
        var service = new ConteudoQueryService(Context);

        var result = await service.ObterAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task ObterAsync_ConteudoSimples_RetornaDadosBasicosEColecoesVazias()
    {
        var usuarioId = Guid.NewGuid();
        var conteudo = Conteudo.Criar(usuarioId, "Duna");
        await Context.Conteudos.AddAsync(conteudo);
        await Context.SaveChangesAsync();

        var service = new ConteudoQueryService(Context);
        var result = await service.ObterAsync(conteudo.Id, usuarioId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Duna", result.Titulo);
        Assert.Equal(conteudo.Id, result.Id);
        Assert.Empty(result.Categorias);
        Assert.Empty(result.Relacoes);
        Assert.Empty(result.Sessoes);
        Assert.Equal(0, result.SessoesContagem);
    }

    [Fact]
    public async Task ObterAsync_ComCategorias_RetornaCategoriasAssociadas()
    {
        var usuarioId = Guid.NewGuid();
        var conteudo = Conteudo.Criar(usuarioId, "Fundação");
        await Context.Conteudos.AddAsync(conteudo);

        var categoria = Categoria.Criar(usuarioId, "Ficção Científica");
        await Context.Categorias.AddAsync(categoria);

        await Context.ConteudoCategorias.AddAsync(new ConteudoCategoria
        {
            ConteudoId = conteudo.Id,
            CategoriaId = categoria.Id,
            AssociadaEm = DateTimeOffset.UtcNow,
        });
        await Context.SaveChangesAsync();

        var service = new ConteudoQueryService(Context);
        var result = await service.ObterAsync(conteudo.Id, usuarioId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Categorias);
        Assert.Equal(categoria.Id, result.Categorias[0].Id);
        Assert.Equal("Ficção Científica", result.Categorias[0].Nome);
    }

    [Fact]
    public async Task ObterAsync_ComRelacoes_ExcluiRelacoesTipoContem()
    {
        var usuarioId = Guid.NewGuid();
        var origem = Conteudo.Criar(usuarioId, "Duna");
        var destino = Conteudo.Criar(usuarioId, "Dune Messiah");
        await Context.Conteudos.AddRangeAsync(origem, destino);

        var (fwd, inv) = BuildPar(origem.Id, destino.Id, _sequenciaTipoId, usuarioId);
        await Context.Relacoes.AddRangeAsync(fwd, inv);
        await Context.SaveChangesAsync();

        var service = new ConteudoQueryService(Context);
        var result = await service.ObterAsync(origem.Id, usuarioId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Relacoes);
        Assert.Equal(destino.Id, result.Relacoes[0].ConteudoDestinoId);
        Assert.Equal("Dune Messiah", result.Relacoes[0].TituloDestino);
    }

    [Fact]
    public async Task ObterAsync_ComSessoes_RetornaTimelineEContagem()
    {
        var usuarioId = Guid.NewGuid();
        var pai = Conteudo.Criar(usuarioId, "Breaking Bad");
        var ep1 = Conteudo.CriarComoFilho(usuarioId, "S01E01");
        await Context.Conteudos.AddRangeAsync(pai, ep1);

        var (fwd, inv) = BuildPar(pai.Id, ep1.Id, _contemTipoId, usuarioId);
        await Context.Relacoes.AddRangeAsync(fwd, inv);
        await Context.SaveChangesAsync();

        var service = new ConteudoQueryService(Context);
        var result = await service.ObterAsync(pai.Id, usuarioId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Sessoes);
        Assert.Equal("S01E01", result.Sessoes[0].Titulo);
        Assert.Equal(1, result.SessoesContagem);
        Assert.Empty(result.Relacoes); // Contém type excluded from relacoes
    }

    [Fact]
    public async Task ListarSessoesAsync_RetornaSessoesPaginadasOrdenadas()
    {
        var usuarioId = Guid.NewGuid();
        var pai = Conteudo.Criar(usuarioId, "The Wire");
        var ep1 = Conteudo.CriarComoFilho(usuarioId, "S01E01", dataConsumo: DateTimeOffset.UtcNow.AddDays(-2));
        var ep2 = Conteudo.CriarComoFilho(usuarioId, "S01E02", dataConsumo: DateTimeOffset.UtcNow.AddDays(-1));
        await Context.Conteudos.AddRangeAsync(pai, ep1, ep2);

        var (fwd1, inv1) = BuildPar(pai.Id, ep1.Id, _contemTipoId, usuarioId);
        var (fwd2, inv2) = BuildPar(pai.Id, ep2.Id, _contemTipoId, usuarioId);
        await Context.Relacoes.AddRangeAsync(fwd1, inv1, fwd2, inv2);
        await Context.SaveChangesAsync();

        var service = new ConteudoQueryService(Context);
        var result = await service.ListarSessoesAsync(pai.Id, usuarioId, new PaginacaoParams(1, 10), CancellationToken.None);

        Assert.Equal(2, result.TotalItems);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task ListarSessoesAsync_UsuarioErrado_RetornaVazio()
    {
        var usuarioId = Guid.NewGuid();
        var outroUsuario = Guid.NewGuid();
        var pai = Conteudo.Criar(usuarioId, "Breaking Bad");
        await Context.Conteudos.AddAsync(pai);
        await Context.SaveChangesAsync();

        var service = new ConteudoQueryService(Context);
        var result = await service.ListarSessoesAsync(pai.Id, outroUsuario, PaginacaoParams.Padrao, CancellationToken.None);

        Assert.Equal(0, result.TotalItems);
        Assert.Empty(result.Items);
    }
}
