using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Core.Enums;
using DiarioDeBordo.Core.Repositorios;
using DiarioDeBordo.Module.Acervo.Commands;
using NSubstitute;

namespace DiarioDeBordo.Tests.Domain.Acervo;

public class FonteCommandHandlerTests
{
    private readonly IConteudoRepository _repo = Substitute.For<IConteudoRepository>();
    private readonly Guid _usuarioId = Guid.NewGuid();

    [Fact]
    public async Task Given_ConteudoExistente_When_AdicionarFonte_Then_FonteAdicionada()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "Item");
        var conteudoId = Guid.NewGuid();
        _repo.ObterPorIdAsync(conteudoId, _usuarioId, Arg.Any<CancellationToken>()).Returns(conteudo);
        var handler = new AdicionarFonteHandler(_repo);

        var resultado = await handler.Handle(
            new AdicionarFonteCommand(conteudoId, _usuarioId, TipoFonte.Url, "https://example.com"),
            CancellationToken.None);

        Assert.True(resultado.IsSuccess);
        Assert.Single(conteudo.Fontes);
        Assert.Equal(1, conteudo.Fontes[0].Prioridade);
    }

    [Fact]
    public async Task Given_ConteudoExistente_When_RemoverFonte_Then_FonteRemovida()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "Item");
        conteudo.AdicionarFonte(TipoFonte.Url, "https://a.com", 1);
        var fonteId = conteudo.Fontes[0].Id;
        var conteudoId = Guid.NewGuid();
        _repo.ObterPorIdAsync(conteudoId, _usuarioId, Arg.Any<CancellationToken>()).Returns(conteudo);
        var handler = new RemoverFonteHandler(_repo);

        var resultado = await handler.Handle(
            new RemoverFonteCommand(conteudoId, _usuarioId, fonteId),
            CancellationToken.None);

        Assert.True(resultado.IsSuccess);
        Assert.Empty(conteudo.Fontes);
    }

    [Fact]
    public async Task Given_ConteudoExistente_When_ReordenarFontes_Then_PrioridadesAtualizadas()
    {
        var conteudo = Conteudo.Criar(_usuarioId, "Item");
        conteudo.AdicionarFonte(TipoFonte.Url, "https://a.com", 1);
        conteudo.AdicionarFonte(TipoFonte.Rss, "https://b.com/rss", 2);
        var primeira = conteudo.Fontes[0].Id;
        var segunda = conteudo.Fontes[1].Id;
        var conteudoId = Guid.NewGuid();
        _repo.ObterPorIdAsync(conteudoId, _usuarioId, Arg.Any<CancellationToken>()).Returns(conteudo);
        var handler = new ReordenarFontesHandler(_repo);

        var resultado = await handler.Handle(
            new ReordenarFontesCommand(conteudoId, _usuarioId, [segunda, primeira]),
            CancellationToken.None);

        Assert.True(resultado.IsSuccess);
        Assert.Equal(1, conteudo.Fontes.Single(f => f.Id == segunda).Prioridade);
        Assert.Equal(2, conteudo.Fontes.Single(f => f.Id == primeira).Prioridade);
    }
}
