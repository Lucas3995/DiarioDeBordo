using DiarioDeBordo.Application.Status;
using FluentAssertions;
using Moq;

namespace DiarioDeBordo.Tests.Unit;

/// <summary>
/// Testes unitários para GetStatusQueryHandler.
/// Segue ciclo TDD (Red-Green-Refactor) e padrão AAA (Arrange-Act-Assert).
/// Isolado via Mock — não depende de banco, rede ou relógio externo.
/// </summary>
public sealed class GetStatusQueryHandlerTests
{
    private readonly Mock<IAmbiente> _ambienteMock;
    private readonly GetStatusQueryHandler _handler;

    public GetStatusQueryHandlerTests()
    {
        _ambienteMock = new Mock<IAmbiente>();
        _handler = new GetStatusQueryHandler(_ambienteMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarStatusComVersaoCorreta()
    {
        // Arrange
        _ambienteMock.Setup(a => a.NomeAmbiente).Returns("Development");
        var query = new GetStatusQuery();

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Versao.Should().Be("1.0.0");
    }

    [Fact]
    public async Task Handle_DeveRetornarAmbienteCorreto()
    {
        // Arrange
        _ambienteMock.Setup(a => a.NomeAmbiente).Returns("Production");
        var query = new GetStatusQuery();

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.Ambiente.Should().Be("Production");
    }

    [Fact]
    public async Task Handle_DeveRetornarHoraServidorProximaDeUtcNow()
    {
        // Arrange
        _ambienteMock.Setup(a => a.NomeAmbiente).Returns("Development");
        var antes = DateTime.UtcNow;
        var query = new GetStatusQuery();

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.HoraServidor.Should().BeOnOrAfter(antes).And.BeOnOrBefore(DateTime.UtcNow.AddSeconds(1));
    }
}
