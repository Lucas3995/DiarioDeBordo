using DiarioDeBordo.Application.Auth.Login;
using DiarioDeBordo.Domain.Auth;
using FluentAssertions;
using Moq;
using Xunit;

// handler agora depende de IAuthenticationService
using DiarioDeBordo.Application.Auth;

namespace DiarioDeBordo.Tests.Unit.Auth;

public sealed class LoginCommandHandlerTests
{
    private readonly Mock<IAuthenticationService> _authServiceMock = new();

    private LoginCommandHandler CriarHandler() =>
        new(_authServiceMock.Object);

    private static LoginResponse SucessoGerado()
    {
        return new LoginResponse(
            Token: "tok", ExpiresAt: DateTime.UtcNow.AddHours(1),
            Requer2FA: false, Sucesso: true, Erro: null);
    }

    // ───────────────────────── Credenciais válidas, sem 2FA ─────────────────────────

    [Fact]
    public async Task Handle_DelegatesToService_ReturnsResult()
    {
        // Arrange
        var expected = SucessoGerado();
        _authServiceMock.Setup(s => s.Authenticate(It.IsAny<LoginCommand>(), default))
                        .ReturnsAsync(expected);

        var handler = CriarHandler();

        // Act
        var result = await handler.Handle(new LoginCommand("admin", "senha"), default);

        // Assert
        result.Should().BeSameAs(expected);
        _authServiceMock.Verify(s => s.Authenticate(It.IsAny<LoginCommand>(), default), Times.Once);
    }
}

    