using DiarioDeBordo.Application.Obras;
using DiarioDeBordo.Application.Obras.ObterPorIdOuNome;
using DiarioDeBordo.Domain.Obras;
using FluentAssertions;
using Moq;

namespace DiarioDeBordo.Tests.Unit.Obras;

/// <summary>
/// Testes unitários para GetObraPorIdOuNomeQueryHandler.
/// Relatório item 6: prévia — obter obra atual por id ou nome.
/// </summary>
public sealed class GetObraPorIdOuNomeQueryHandlerTests
{
    private readonly Mock<IObraEscritaRepository> _repoMock;
    private readonly GetObraPorIdOuNomeQueryHandler _handler;

    public GetObraPorIdOuNomeQueryHandlerTests()
    {
        _repoMock = new Mock<IObraEscritaRepository>();
        _handler = new GetObraPorIdOuNomeQueryHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ComIdExistente_DeveRetornarDtoDaObra()
    {
        // Arrange
        var data = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc);
        var obra = new Obra("One Piece", TipoObra.Manga, 1110, data, 1);
        _repoMock.Setup(r => r.ObterPorIdAsync(obra.Id, It.IsAny<CancellationToken>())).ReturnsAsync(obra);
        var query = new GetObraPorIdOuNomeQuery(Id: obra.Id, Nome: null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(obra.Id);
        result.Nome.Should().Be("One Piece");
        result.Tipo.Should().Be(TipoObra.Manga);
        result.PosicaoAtual.Should().Be(1110);
        result.DataUltimaAtualizacaoPosicao.Should().Be(data);
        result.OrdemPreferencia.Should().Be(1);
    }

    [Fact]
    public async Task Handle_ComNomeExistente_DeveRetornarDtoDaObra()
    {
        // Arrange
        var obra = new Obra("Solo Leveling", TipoObra.Manhwa, 179, DateTime.UtcNow, 2);
        _repoMock.Setup(r => r.ObterPorNomeAsync("Solo Leveling", It.IsAny<CancellationToken>())).ReturnsAsync(obra);
        var query = new GetObraPorIdOuNomeQuery(Id: null, Nome: "Solo Leveling");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Nome.Should().Be("Solo Leveling");
        result.PosicaoAtual.Should().Be(179);
    }

    [Fact]
    public async Task Handle_ObraNaoEncontrada_DeveRetornarNull()
    {
        _repoMock.Setup(r => r.ObterPorNomeAsync("Inexistente", It.IsAny<CancellationToken>())).ReturnsAsync((Obra?)null);
        var query = new GetObraPorIdOuNomeQuery(Id: null, Nome: "Inexistente");

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();
    }
}
