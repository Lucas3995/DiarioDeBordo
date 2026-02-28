using DiarioDeBordo.Application.Auth;
using DiarioDeBordo.Application.Auth.Login;
using DiarioDeBordo.Domain.Auth;
using FluentAssertions;
using Moq;
using Xunit;

namespace DiarioDeBordo.Tests.Unit.Auth;

public sealed class LoginCommandHandlerTests
{
    private readonly Mock<IUsuarioRepository> _repoMock = new();
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();

    private LoginCommandHandler CriarHandler() =>
        new(_repoMock.Object, _tokenServiceMock.Object, _passwordHasherMock.Object);

    private static Usuario CriarAdmin(bool ativo = true, bool requer2FA = false)
    {
        var u = new Usuario("admin", Perfil.Admin);
        u.DefinirSenhaHash("$2a$12$hashdoAdmin");
        if (!ativo) u.Desativar();
        if (requer2FA) u.HabilitarSegundoFator();
        return u;
    }

    // ───────────────────────── Credenciais válidas, sem 2FA ─────────────────────────

    [Fact]
    public async Task Handle_CredenciaisValidas_SucedoTrueTokenNaoNuloExpiresAtFuturo()
    {
        // Arrange
        var usuario = CriarAdmin();
        var expiresAt = DateTime.UtcNow.AddHours(8);

        _repoMock.Setup(r => r.BuscarPorLoginAsync("admin", default))
                 .ReturnsAsync(usuario);
        _passwordHasherMock.Setup(p => p.Verificar("camaradinha@123", usuario.SenhaHash))
                           .Returns(true);
        _tokenServiceMock.Setup(t => t.GerarToken(usuario))
                         .Returns(("jwt.token.valido", expiresAt));

        var handler = CriarHandler();

        // Act
        var result = await handler.Handle(new LoginCommand("admin", "camaradinha@123"), default);

        // Assert
        result.Sucesso.Should().BeTrue();
        result.Token.Should().NotBeNullOrWhiteSpace();
        result.ExpiresAt.Should().NotBeNull()
              .And.BeAfter(DateTime.UtcNow);
        result.Requer2FA.Should().BeFalse();
        result.Erro.Should().BeNull();
    }

    [Fact]
    public async Task Handle_CredenciaisValidas_GerarTokenChamadoExatamenteUmaVez()
    {
        // Arrange
        var usuario = CriarAdmin();

        _repoMock.Setup(r => r.BuscarPorLoginAsync("admin", default)).ReturnsAsync(usuario);
        _passwordHasherMock.Setup(p => p.Verificar(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        _tokenServiceMock.Setup(t => t.GerarToken(usuario)).Returns(("tok", DateTime.UtcNow.AddHours(1)));

        var handler = CriarHandler();

        // Act
        await handler.Handle(new LoginCommand("admin", "camaradinha@123"), default);

        // Assert — token deve ser gerado exatamente 1 vez no caso de sucesso
        _tokenServiceMock.Verify(t => t.GerarToken(usuario), Times.Once);
    }

    [Fact]
    public async Task Handle_CredenciaisValidas_PasswordHasherChamadoComSenhaESenhaHash()
    {
        // Arrange
        const string senhaClara = "camaradinha@123";
        var usuario = CriarAdmin();

        _repoMock.Setup(r => r.BuscarPorLoginAsync("admin", default)).ReturnsAsync(usuario);
        _passwordHasherMock.Setup(p => p.Verificar(senhaClara, usuario.SenhaHash)).Returns(true);
        _tokenServiceMock.Setup(t => t.GerarToken(usuario)).Returns(("tok", DateTime.UtcNow.AddHours(1)));

        var handler = CriarHandler();

        // Act
        await handler.Handle(new LoginCommand("admin", senhaClara), default);

        // Assert — verifica que a senha e o hash corretos foram passados ao hasher
        _passwordHasherMock.Verify(p => p.Verificar(senhaClara, usuario.SenhaHash), Times.Once);
    }

    // ───────────────────────── Login inexistente ─────────────────────────

    [Fact]
    public async Task Handle_LoginInexistente_RetornaFalhaComMensagemGenerica()
    {
        // Arrange
        _repoMock.Setup(r => r.BuscarPorLoginAsync("naoexiste", default))
                 .ReturnsAsync((Usuario?)null);
        _passwordHasherMock.Setup(p => p.Verificar(It.IsAny<string>(), It.IsAny<string>()))
                           .Returns(false);

        var handler = CriarHandler();

        // Act
        var result = await handler.Handle(new LoginCommand("naoexiste", "qualquer"), default);

        // Assert
        result.Sucesso.Should().BeFalse();
        result.Token.Should().BeNull();
        result.Erro.Should().Be("Credenciais inválidas.");
    }

    [Fact]
    public async Task Handle_LoginInexistente_NaoChama_GerarToken()
    {
        // Arrange
        _repoMock.Setup(r => r.BuscarPorLoginAsync(It.IsAny<string>(), default))
                 .ReturnsAsync((Usuario?)null);
        _passwordHasherMock.Setup(p => p.Verificar(It.IsAny<string>(), It.IsAny<string>()))
                           .Returns(false);

        var handler = CriarHandler();

        // Act
        await handler.Handle(new LoginCommand("naoexiste", "qualquer"), default);

        // Assert
        _tokenServiceMock.Verify(t => t.GerarToken(It.IsAny<Usuario>()), Times.Never);
    }

    // ───────────────────────── Senha incorreta ─────────────────────────

    [Fact]
    public async Task Handle_SenhaIncorreta_RetornaMesmaMensagemQueLoginInexistente()
    {
        // Arrange — proteger contra enumeração: mensagem deve ser idêntica
        var usuario = CriarAdmin();

        _repoMock.Setup(r => r.BuscarPorLoginAsync("admin", default)).ReturnsAsync(usuario);
        _passwordHasherMock.Setup(p => p.Verificar("senhaErrada", usuario.SenhaHash)).Returns(false);

        var handler = CriarHandler();

        // Act (senha errada)
        var resultSenhaErrada = await handler.Handle(new LoginCommand("admin", "senhaErrada"), default);

        // Reset mock para simular login inexistente
        _repoMock.Setup(r => r.BuscarPorLoginAsync("naoexiste", default)).ReturnsAsync((Usuario?)null);
        _passwordHasherMock.Setup(p => p.Verificar(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
        var resultLoginInexistente = await handler.Handle(new LoginCommand("naoexiste", "qualquer"), default);

        // Assert — mesma mensagem (anti-enumeração)
        resultSenhaErrada.Erro.Should().Be(resultLoginInexistente.Erro);
        resultSenhaErrada.Sucesso.Should().BeFalse();
        resultSenhaErrada.Token.Should().BeNull();
    }

    // ───────────────────────── Usuário inativo ─────────────────────────

    [Fact]
    public async Task Handle_UsuarioInativo_RetornaFalhaTokenNulo()
    {
        // Arrange
        var usuarioInativo = CriarAdmin(ativo: false);

        _repoMock.Setup(r => r.BuscarPorLoginAsync("admin", default)).ReturnsAsync(usuarioInativo);
        _passwordHasherMock.Setup(p => p.Verificar(It.IsAny<string>(), It.IsAny<string>()))
                           .Returns(true); // mesmo com senha "correta", deve bloquear

        var handler = CriarHandler();

        // Act
        var result = await handler.Handle(new LoginCommand("admin", "camaradinha@123"), default);

        // Assert
        result.Sucesso.Should().BeFalse();
        result.Token.Should().BeNull();
        _tokenServiceMock.Verify(t => t.GerarToken(It.IsAny<Usuario>()), Times.Never);
    }

    // ───────────────────────── Usuário com Requer2FA = true ─────────────────────────

    [Fact]
    public async Task Handle_UsuarioComRequer2FA_RetornaRequer2FATrueTokenNulo()
    {
        // Arrange
        var usuarioCom2FA = CriarAdmin(requer2FA: true);

        _repoMock.Setup(r => r.BuscarPorLoginAsync("admin", default)).ReturnsAsync(usuarioCom2FA);
        _passwordHasherMock.Setup(p => p.Verificar("camaradinha@123", usuarioCom2FA.SenhaHash))
                           .Returns(true);

        var handler = CriarHandler();

        // Act
        var result = await handler.Handle(new LoginCommand("admin", "camaradinha@123"), default);

        // Assert
        result.Requer2FA.Should().BeTrue();
        result.Token.Should().BeNull();
        result.Sucesso.Should().BeFalse();
        _tokenServiceMock.Verify(t => t.GerarToken(It.IsAny<Usuario>()), Times.Never);
    }
}
