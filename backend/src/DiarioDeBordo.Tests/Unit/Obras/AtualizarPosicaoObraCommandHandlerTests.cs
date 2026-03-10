using DiarioDeBordo.Application.Obras.AtualizarPosicao;
using DiarioDeBordo.Domain.Common;
using DiarioDeBordo.Domain.Obras;
using FluentAssertions;
using Moq;

namespace DiarioDeBordo.Tests.Unit.Obras;

/// <summary>
/// Testes unitários para AtualizarPosicaoObraCommandHandler.
/// Relatório item 4: atualizar posição por id ou nome; criar obra se não existir.
/// </summary>
public sealed class AtualizarPosicaoObraCommandHandlerTests
{
    private readonly Mock<IObraEscritaRepository> _repoMock;
    private readonly IClock _clock = Mock.Of<IClock>(c => c.UtcNow == new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc));
    private readonly AtualizarPosicaoObraCommandHandler _handler;

    public AtualizarPosicaoObraCommandHandlerTests()
    {
        _repoMock = new Mock<IObraEscritaRepository>();
        _handler = new AtualizarPosicaoObraCommandHandler(_repoMock.Object, _clock);
    }

    [Fact]
    public async Task Handle_ComIdObraExistente_DeveAtualizarPosicaoEPersistir()
    {
        // Arrange
        var obra = new Obra("One Piece", TipoObra.Manga, 1100, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), 1, _clock);
        _repoMock.Setup(r => r.ObterPorIdAsync(obra.Id, It.IsAny<CancellationToken>())).ReturnsAsync(obra);
        var novaData = new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc);
        var command = new AtualizarPosicaoObraCommand(IdObra: obra.Id, NomeObra: null, NovaPosicao: 1115, DataUltimaAtualizacao: novaData, CriarSeNaoExistir: false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Id.Should().Be(obra.Id);
        result.Criada.Should().BeFalse();
        obra.PosicaoAtual.Should().Be(1115);
        obra.DataUltimaAtualizacaoPosicao.Should().Be(novaData);
        _repoMock.Verify(r => r.AtualizarAsync(obra, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ComNomeObraExistente_DeveAtualizarPosicaoEPersistir()
    {
        // Arrange
        var obra = new Obra("Solo Leveling", TipoObra.Manhwa, 179, DateTime.UtcNow, 1, _clock);
        _repoMock.Setup(r => r.ObterPorNomeAsync("Solo Leveling", It.IsAny<CancellationToken>())).ReturnsAsync(obra);
        var command = new AtualizarPosicaoObraCommand(IdObra: null, NomeObra: "Solo Leveling", NovaPosicao: 180, DataUltimaAtualizacao: null, CriarSeNaoExistir: false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Id.Should().Be(obra.Id);
        result.Criada.Should().BeFalse();
        obra.PosicaoAtual.Should().Be(180);
        _repoMock.Verify(r => r.AtualizarAsync(obra, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ObraNaoExisteECriarSeNaoExistir_DeveCriarNovaObra()
    {
        // Arrange
        _repoMock.Setup(r => r.ObterPorNomeAsync("Nova Obra", It.IsAny<CancellationToken>())).ReturnsAsync((Obra?)null);
        Obra? obraAdicionada = null;
        _repoMock.Setup(r => r.AdicionarAsync(It.IsAny<Obra>(), It.IsAny<CancellationToken>()))
            .Callback<Obra, CancellationToken>((o, _) => obraAdicionada = o)
            .Returns(Task.CompletedTask);
        var command = new AtualizarPosicaoObraCommand(
            IdObra: null, NomeObra: "Nova Obra", NovaPosicao: 1, DataUltimaAtualizacao: null, CriarSeNaoExistir: true,
            NomeParaCriar: "Nova Obra", TipoParaCriar: TipoObra.Manga, OrdemPreferenciaParaCriar: 99);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Criada.Should().BeTrue();
        obraAdicionada.Should().NotBeNull();
        obraAdicionada!.Nome.Should().Be("Nova Obra");
        obraAdicionada.Tipo.Should().Be(TipoObra.Manga);
        obraAdicionada.PosicaoAtual.Should().Be(1);
        obraAdicionada.OrdemPreferencia.Should().Be(99);
        result.Id.Should().Be(obraAdicionada.Id);
    }

    [Fact]
    public async Task Handle_ObraNaoExisteSemCriar_DeveLancarExcecao()
    {
        // Arrange
        _repoMock.Setup(r => r.ObterPorNomeAsync("Inexistente", It.IsAny<CancellationToken>())).ReturnsAsync((Obra?)null);
        var command = new AtualizarPosicaoObraCommand(IdObra: null, NomeObra: "Inexistente", NovaPosicao: 1, DataUltimaAtualizacao: null, CriarSeNaoExistir: false);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
