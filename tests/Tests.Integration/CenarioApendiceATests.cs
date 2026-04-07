using DiarioDeBordo.Core.Enums;
using DiarioDeBordo.Core.FeatureFlags;
using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Infrastructure;
using DiarioDeBordo.Infrastructure.Persistencia;
using DiarioDeBordo.Module.Acervo.Commands;
using DiarioDeBordo.Module.Acervo.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;
using Xunit;

namespace DiarioDeBordo.Tests.Integration;

public sealed class CenarioApendiceATests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("diariodebordo_integration_appendix_a")
        .WithUsername("integration_user")
        .WithPassword("integration_password_secure")
        .Build();

    private IServiceProvider _services = null!;
    private readonly Guid _usuarioId = Guid.NewGuid();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        var services = new ServiceCollection();

        services.AddInfrastructure(_postgres.GetConnectionString());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CriarConteudoCommand).Assembly);
        });

        services.AddSingleton<IFeatureFlags, FeatureFlagsPlaceholder>();
        services.AddLogging(l => l.SetMinimumLevel(LogLevel.Warning));

        _services = services.BuildServiceProvider();

        using var scope = _services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<DiarioDeBordoDbContext>();
        await ctx.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        if (_services is IAsyncDisposable asyncDisposable)
            await asyncDisposable.DisposeAsync();

        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task Cenario1_ListasFilmesTematicasComAnotacoesContextuais()
    {
        using var scope = _services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var coletaneaResult = await mediator.Send(new CriarConteudoCommand(
            _usuarioId,
            "Filmes de Terror para Noites Chuvosas",
            Papel: PapelConteudo.Coletanea,
            TipoColetanea: TipoColetanea.Miscelanea,
            Formato: FormatoMidia.Video));
        Assert.True(coletaneaResult.IsSuccess);
        var coletaneaId = coletaneaResult.Value;

        var filme1Result = await mediator.Send(new CriarConteudoCommand(
            _usuarioId,
            "O Iluminado",
            Anotacoes: "Anotação global: clássico",
            Formato: FormatoMidia.Video));
        Assert.True(filme1Result.IsSuccess);

        var filme2Result = await mediator.Send(new CriarConteudoCommand(
            _usuarioId,
            "O Exorcista",
            Formato: FormatoMidia.Video));
        Assert.True(filme2Result.IsSuccess);

        var add1 = await mediator.Send(new AdicionarItemNaColetaneaCommand(coletaneaId, filme1Result.Value, _usuarioId));
        var add2 = await mediator.Send(new AdicionarItemNaColetaneaCommand(coletaneaId, filme2Result.Value, _usuarioId));
        Assert.True(add1.IsSuccess);
        Assert.True(add2.IsSuccess);

        var anot1 = await mediator.Send(new AtualizarAnotacaoContextualCommand(
            coletaneaId,
            filme1Result.Value,
            _usuarioId,
            "não achei dublado"));
        var anot2 = await mediator.Send(new AtualizarAnotacaoContextualCommand(
            coletaneaId,
            filme2Result.Value,
            _usuarioId,
            "assistir em dia chuvoso"));
        Assert.True(anot1.IsSuccess);
        Assert.True(anot2.IsSuccess);

        var itensResult = await mediator.Send(new ListarItensColetaneaQuery(
            coletaneaId,
            _usuarioId,
            new PaginacaoParams(1, 10)));
        Assert.True(itensResult.IsSuccess);
        Assert.Equal(2, itensResult.Value!.TotalItems);

        var item1 = itensResult.Value.Items.Single(i => i.Titulo == "O Iluminado");
        var item2 = itensResult.Value.Items.Single(i => i.Titulo == "O Exorcista");

        Assert.Equal("não achei dublado", item1.AnotacaoContextual);
        Assert.Equal("assistir em dia chuvoso", item2.AnotacaoContextual);

        var filme1Detalhe = await mediator.Send(new ObterConteudoQuery(filme1Result.Value, _usuarioId));
        Assert.True(filme1Detalhe.IsSuccess);
        Assert.Equal("Anotação global: clássico", filme1Detalhe.Value!.Anotacoes);
    }

    [Fact]
    public async Task Cenario2_CDsComFallbackParaSpotifyOuYouTube()
    {
        using var scope = _services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var playlistResult = await mediator.Send(new CriarConteudoCommand(
            _usuarioId,
            "Minha Playlist de CDs",
            Papel: PapelConteudo.Coletanea,
            TipoColetanea: TipoColetanea.Miscelanea,
            Formato: FormatoMidia.Audio));
        Assert.True(playlistResult.IsSuccess);

        var musicas = new[]
        {
            "Bohemian Rhapsody",
            "Smells Like Teen Spirit",
            "Hotel California"
        };

        foreach (var musica in musicas)
        {
            var musicaResult = await mediator.Send(new CriarConteudoCommand(
                _usuarioId,
                musica,
                Formato: FormatoMidia.Audio));
            Assert.True(musicaResult.IsSuccess);

            var localResult = await mediator.Send(new AdicionarFonteCommand(
                musicaResult.Value,
                _usuarioId,
                TipoFonte.ArquivoLocal,
                $"/mnt/cds/{musica.Replace(' ', '_').ToUpperInvariant()}.flac",
                "Local"));
            var spotifyResult = await mediator.Send(new AdicionarFonteCommand(
                musicaResult.Value,
                _usuarioId,
                TipoFonte.Url,
                $"https://open.spotify.com/track/{Guid.NewGuid():N}",
                "Spotify"));
            var youtubeResult = await mediator.Send(new AdicionarFonteCommand(
                musicaResult.Value,
                _usuarioId,
                TipoFonte.Url,
                $"https://youtube.com/watch?v={Guid.NewGuid():N}"[..43],
                "YouTube"));

            Assert.True(localResult.IsSuccess);
            Assert.True(spotifyResult.IsSuccess);
            Assert.True(youtubeResult.IsSuccess);

            var addToPlaylist = await mediator.Send(new AdicionarItemNaColetaneaCommand(
                playlistResult.Value,
                musicaResult.Value,
                _usuarioId));
            Assert.True(addToPlaylist.IsSuccess);

            var detalhe = await mediator.Send(new ObterConteudoQuery(musicaResult.Value, _usuarioId));
            Assert.True(detalhe.IsSuccess);
            Assert.Equal(3, detalhe.Value!.Fontes.Count);
            Assert.Equal(1, detalhe.Value.Fontes[0].Prioridade);
            Assert.Equal("ArquivoLocal", detalhe.Value.Fontes[0].Tipo);
            Assert.Equal(2, detalhe.Value.Fontes[1].Prioridade);
            Assert.Equal("Spotify", detalhe.Value.Fontes[1].Plataforma);
            Assert.Equal(3, detalhe.Value.Fontes[2].Prioridade);
            Assert.Equal("YouTube", detalhe.Value.Fontes[2].Plataforma);
        }

        var itensPlaylist = await mediator.Send(new ListarItensColetaneaQuery(
            playlistResult.Value,
            _usuarioId,
            new PaginacaoParams(1, 10)));
        Assert.True(itensPlaylist.IsSuccess);
        Assert.Equal(3, itensPlaylist.Value!.TotalItems);
    }

    [Fact]
    public async Task Cenario3_CadernoFisicoDeDesenhos()
    {
        using var scope = _services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var cadernoResult = await mediator.Send(new CriarConteudoCommand(
            _usuarioId,
            "Caderno de Desenhos 2024",
            Descricao: "Caderno Moleskine A5",
            Anotacoes: "Desenhei até a página 30",
            Formato: FormatoMidia.Nenhum));
        Assert.True(cadernoResult.IsSuccess);

        var imagemResult = await mediator.Send(new AdicionarImagemCapaCommand(
            cadernoResult.Value,
            _usuarioId,
            "/photos/caderno_capa.jpg",
            500_000));
        Assert.True(imagemResult.IsSuccess);

        var conteudoResult = await mediator.Send(new ObterConteudoQuery(cadernoResult.Value, _usuarioId));
        Assert.True(conteudoResult.IsSuccess);
        Assert.Equal("Nenhum", conteudoResult.Value!.Formato);
        Assert.Empty(conteudoResult.Value.Fontes);
        Assert.Single(conteudoResult.Value.Imagens);
        Assert.Equal("/photos/caderno_capa.jpg", conteudoResult.Value.Imagens[0].Caminho);
        Assert.Contains("página 30", conteudoResult.Value.Anotacoes);
    }

    [Fact]
    public async Task Cenario4_PlanoDeEstudoComColetaneasAninhadas()
    {
        using var scope = _services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var cyberResult = await mediator.Send(new CriarConteudoCommand(
            _usuarioId,
            "Fundamentos de Cybersecurity",
            Papel: PapelConteudo.Coletanea,
            TipoColetanea: TipoColetanea.Guiada,
            Formato: FormatoMidia.Texto));
        Assert.True(cyberResult.IsSuccess);

        var artigosCyber = new[]
        {
            "OWASP Top 10",
            "SQL Injection Prevention",
            "Zero Trust Architecture"
        };

        foreach (var artigo in artigosCyber)
        {
            var artigoResult = await mediator.Send(new CriarConteudoCommand(_usuarioId, artigo, Formato: FormatoMidia.Texto));
            Assert.True(artigoResult.IsSuccess);
            var add = await mediator.Send(new AdicionarItemNaColetaneaCommand(cyberResult.Value, artigoResult.Value, _usuarioId));
            Assert.True(add.IsSuccess);
        }

        var planoResult = await mediator.Send(new CriarConteudoCommand(
            _usuarioId,
            "Plano de Estudo: Programação",
            Papel: PapelConteudo.Coletanea,
            TipoColetanea: TipoColetanea.Guiada,
            Formato: FormatoMidia.Misto));
        Assert.True(planoResult.IsSuccess);

        var artigoMain = await mediator.Send(new CriarConteudoCommand(_usuarioId, "SOLID Principles", Formato: FormatoMidia.Texto));
        var videoMain = await mediator.Send(new CriarConteudoCommand(_usuarioId, "Clean Architecture Video", Formato: FormatoMidia.Video));
        var postMain = await mediator.Send(new CriarConteudoCommand(_usuarioId, "Refactoring Checklist", Formato: FormatoMidia.Misto));
        Assert.True(artigoMain.IsSuccess);
        Assert.True(videoMain.IsSuccess);
        Assert.True(postMain.IsSuccess);

        Assert.True((await mediator.Send(new AdicionarItemNaColetaneaCommand(planoResult.Value, artigoMain.Value, _usuarioId))).IsSuccess);
        Assert.True((await mediator.Send(new AdicionarItemNaColetaneaCommand(planoResult.Value, videoMain.Value, _usuarioId))).IsSuccess);
        Assert.True((await mediator.Send(new AdicionarItemNaColetaneaCommand(planoResult.Value, postMain.Value, _usuarioId))).IsSuccess);
        Assert.True((await mediator.Send(new AdicionarItemNaColetaneaCommand(planoResult.Value, cyberResult.Value, _usuarioId))).IsSuccess);

        var itensOrdenadosInicial = await mediator.Send(new ListarItensColetaneaQuery(
            planoResult.Value,
            _usuarioId,
            new PaginacaoParams(1, 10)));
        Assert.True(itensOrdenadosInicial.IsSuccess);
        Assert.Equal(4, itensOrdenadosInicial.Value!.TotalItems);
        Assert.Equal("SOLID Principles", itensOrdenadosInicial.Value.Items[0].Titulo);
        Assert.Equal("Clean Architecture Video", itensOrdenadosInicial.Value.Items[1].Titulo);
        Assert.Equal("Refactoring Checklist", itensOrdenadosInicial.Value.Items[2].Titulo);
        Assert.Equal("Fundamentos de Cybersecurity", itensOrdenadosInicial.Value.Items[3].Titulo);
        Assert.Equal(1, itensOrdenadosInicial.Value.Items[0].Posicao);
        Assert.Equal(2, itensOrdenadosInicial.Value.Items[1].Posicao);
        Assert.Equal(3, itensOrdenadosInicial.Value.Items[2].Posicao);
        Assert.Equal(4, itensOrdenadosInicial.Value.Items[3].Posicao);

        var detalheInicial = await mediator.Send(new ObterColetaneaDetalheQuery(planoResult.Value, _usuarioId));
        Assert.True(detalheInicial.IsSuccess);
        Assert.Equal(0m, detalheInicial.Value!.ProgressoPercentual);

        var progressoItem1 = await mediator.Send(new AtualizarConteudoCommand(
            artigoMain.Value,
            _usuarioId,
            "SOLID Principles",
            null,
            null,
            null,
            null,
            FormatoMidia.Texto,
            null,
            EstadoProgresso.Concluido,
            null,
            null,
            []));
        Assert.True(progressoItem1.IsSuccess);

        var detalheAposPrimeiro = await mediator.Send(new ObterColetaneaDetalheQuery(planoResult.Value, _usuarioId));
        Assert.True(detalheAposPrimeiro.IsSuccess);
        Assert.Equal(25m, detalheAposPrimeiro.Value!.ProgressoPercentual);

        var progressoItem2 = await mediator.Send(new AtualizarConteudoCommand(
            videoMain.Value,
            _usuarioId,
            "Clean Architecture Video",
            null,
            null,
            null,
            null,
            FormatoMidia.Video,
            null,
            EstadoProgresso.Concluido,
            null,
            null,
            []));
        Assert.True(progressoItem2.IsSuccess);

        var detalheAposSegundo = await mediator.Send(new ObterColetaneaDetalheQuery(planoResult.Value, _usuarioId));
        Assert.True(detalheAposSegundo.IsSuccess);
        Assert.Equal(50m, detalheAposSegundo.Value!.ProgressoPercentual);

        var detalheResult = await mediator.Send(new ObterColetaneaDetalheQuery(planoResult.Value, _usuarioId));
        var itensResult = await mediator.Send(new ListarItensColetaneaQuery(
            planoResult.Value,
            _usuarioId,
            new PaginacaoParams(1, 10)));

        Assert.True(detalheResult.IsSuccess);
        Assert.Equal("Guiada", detalheResult.Value!.TipoColetanea);
        Assert.Equal(4, detalheResult.Value.QuantidadeItens);

        Assert.True(itensResult.IsSuccess);
        Assert.Equal(4, itensResult.Value!.TotalItems);

        var nestedCollection = itensResult.Value.Items.Single(i => i.Titulo == "Fundamentos de Cybersecurity");
        Assert.Equal("Coletanea", nestedCollection.Papel);
        Assert.Equal("Guiada", nestedCollection.TipoColetanea);
        Assert.Equal(3, nestedCollection.SubItensContagem);
    }

    [Fact]
    public async Task Cenario_ACE05_MetadadoManualPrevaleceSobreAutomatico()
    {
        using var scope = _services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var conteudoResult = await mediator.Send(new CriarConteudoCommand(
            _usuarioId,
            "Faixa de Teste",
            Descricao: "Descricao manual prioritaria",
            Anotacoes: "Anotacao manual prioritaria",
            Formato: FormatoMidia.Audio));
        Assert.True(conteudoResult.IsSuccess);
        var conteudoId = conteudoResult.Value;

        var fonteLocal = await mediator.Send(new AdicionarFonteCommand(
            conteudoId,
            _usuarioId,
            TipoFonte.ArquivoLocal,
            "/mnt/musicas/faixa_teste.flac",
            "Local"));
        var fonteSpotify = await mediator.Send(new AdicionarFonteCommand(
            conteudoId,
            _usuarioId,
            TipoFonte.Url,
            "https://open.spotify.com/track/manual-priority-case",
            "Spotify"));
        var fonteYoutube = await mediator.Send(new AdicionarFonteCommand(
            conteudoId,
            _usuarioId,
            TipoFonte.Url,
            "https://youtube.com/watch?v=manualpriority",
            "YouTube"));

        Assert.True(fonteLocal.IsSuccess);
        Assert.True(fonteSpotify.IsSuccess);
        Assert.True(fonteYoutube.IsSuccess);

        var detalhe = await mediator.Send(new ObterConteudoQuery(conteudoId, _usuarioId));
        Assert.True(detalhe.IsSuccess);
        Assert.Equal("Descricao manual prioritaria", detalhe.Value!.Descricao);
        Assert.Equal("Anotacao manual prioritaria", detalhe.Value.Anotacoes);
        Assert.Equal(3, detalhe.Value.Fontes.Count);
        Assert.Equal("ArquivoLocal", detalhe.Value.Fontes[0].Tipo);
        Assert.Equal("Spotify", detalhe.Value.Fontes[1].Plataforma);
        Assert.Equal("YouTube", detalhe.Value.Fontes[2].Plataforma);
    }

    [Fact]
    public async Task Cenario5_FranquiaResidentEvilComColetaneasAninhadas()
    {
        using var scope = _services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var reResult = await mediator.Send(new CriarConteudoCommand(
            _usuarioId,
            "Resident Evil",
            Papel: PapelConteudo.Coletanea,
            TipoColetanea: TipoColetanea.Guiada,
            Formato: FormatoMidia.Misto));
        Assert.True(reResult.IsSuccess);

        var jogosResult = await mediator.Send(new CriarConteudoCommand(
            _usuarioId,
            "RE — Jogos",
            Papel: PapelConteudo.Coletanea,
            TipoColetanea: TipoColetanea.Guiada,
            Formato: FormatoMidia.Interativo));
        var filmesResult = await mediator.Send(new CriarConteudoCommand(
            _usuarioId,
            "RE — Filmes",
            Papel: PapelConteudo.Coletanea,
            TipoColetanea: TipoColetanea.Guiada,
            Formato: FormatoMidia.Video));
        var livrosResult = await mediator.Send(new CriarConteudoCommand(
            _usuarioId,
            "RE — Livros",
            Papel: PapelConteudo.Coletanea,
            TipoColetanea: TipoColetanea.Guiada,
            Formato: FormatoMidia.Texto));

        Assert.True(jogosResult.IsSuccess);
        Assert.True(filmesResult.IsSuccess);
        Assert.True(livrosResult.IsSuccess);

        var jogo1 = await mediator.Send(new CriarConteudoCommand(_usuarioId, "Resident Evil 1", Formato: FormatoMidia.Interativo));
        var jogo2 = await mediator.Send(new CriarConteudoCommand(_usuarioId, "Resident Evil 2", Formato: FormatoMidia.Interativo));
        var filme1 = await mediator.Send(new CriarConteudoCommand(_usuarioId, "Resident Evil (2002)", Formato: FormatoMidia.Video));
        var filme2 = await mediator.Send(new CriarConteudoCommand(_usuarioId, "Resident Evil: Apocalypse", Formato: FormatoMidia.Video));
        var livro1 = await mediator.Send(new CriarConteudoCommand(_usuarioId, "Resident Evil: The Umbrella Conspiracy", Formato: FormatoMidia.Texto));
        var livro2 = await mediator.Send(new CriarConteudoCommand(_usuarioId, "Resident Evil: Caliban Cove", Formato: FormatoMidia.Texto));

        Assert.True(jogo1.IsSuccess && jogo2.IsSuccess && filme1.IsSuccess && filme2.IsSuccess && livro1.IsSuccess && livro2.IsSuccess);

        Assert.True((await mediator.Send(new AdicionarItemNaColetaneaCommand(jogosResult.Value, jogo1.Value, _usuarioId))).IsSuccess);
        Assert.True((await mediator.Send(new AdicionarItemNaColetaneaCommand(jogosResult.Value, jogo2.Value, _usuarioId))).IsSuccess);
        Assert.True((await mediator.Send(new AdicionarItemNaColetaneaCommand(filmesResult.Value, filme1.Value, _usuarioId))).IsSuccess);
        Assert.True((await mediator.Send(new AdicionarItemNaColetaneaCommand(filmesResult.Value, filme2.Value, _usuarioId))).IsSuccess);
        Assert.True((await mediator.Send(new AdicionarItemNaColetaneaCommand(livrosResult.Value, livro1.Value, _usuarioId))).IsSuccess);
        Assert.True((await mediator.Send(new AdicionarItemNaColetaneaCommand(livrosResult.Value, livro2.Value, _usuarioId))).IsSuccess);

        Assert.True((await mediator.Send(new AdicionarItemNaColetaneaCommand(reResult.Value, jogosResult.Value, _usuarioId))).IsSuccess);
        Assert.True((await mediator.Send(new AdicionarItemNaColetaneaCommand(reResult.Value, filmesResult.Value, _usuarioId))).IsSuccess);
        Assert.True((await mediator.Send(new AdicionarItemNaColetaneaCommand(reResult.Value, livrosResult.Value, _usuarioId))).IsSuccess);

        var detalheResult = await mediator.Send(new ObterColetaneaDetalheQuery(reResult.Value, _usuarioId));
        var itensResult = await mediator.Send(new ListarItensColetaneaQuery(
            reResult.Value,
            _usuarioId,
            new PaginacaoParams(1, 10)));

        Assert.True(detalheResult.IsSuccess);
        Assert.Equal(3, detalheResult.Value!.QuantidadeItens);

        Assert.True(itensResult.IsSuccess);
        Assert.Equal(3, itensResult.Value!.TotalItems);

        var cycleResult = await mediator.Send(new AdicionarItemNaColetaneaCommand(
            jogosResult.Value,
            reResult.Value,
            _usuarioId));

        Assert.True(cycleResult.IsFailure);
        Assert.Equal("CICLO_COMPOSICAO", cycleResult.Error!.Codigo);
    }
}
