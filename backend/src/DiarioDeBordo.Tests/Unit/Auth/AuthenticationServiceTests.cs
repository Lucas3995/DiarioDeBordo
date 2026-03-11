using DiarioDeBordo.Application.Auth;
using DiarioDeBordo.Application.Auth.Login;
using DiarioDeBordo.Domain.Auth;
using FluentAssertions;
using Moq;
using Xunit;

namespace DiarioDeBordo.Tests.Unit.Auth;

public sealed class AuthenticationServiceTests
{
    private readonly Mock<IUsuarioRepository> _repoMock = new();
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();

    private AuthenticationService CriarService() =>
        new(_repoMock.Object, _tokenServiceMock.Object, _passwordHasherMock.Object);

    private static Usuario CriarAdmin(bool ativo = true, bool requer2FA = false)
    {
        var u = new Usuario("admin", Perfil.Admin);
        u.DefinirSenhaHash("$2a$12$hashdoAdmin");
        if (!ativo) u.Desativar();
        if (requer2FA) u.HabilitarSegundoFator();
        return u;
    }

    // reproduz os mesmos cenários antigos, agora diretamente no serviço

    [Fact]
    public async Task Authenticate_CredenciaisValidas_SucessoTrueTokenNaoNuloExpiresAtFuturo()
    {
        var usuario = CriarAdmin();
        var expiresAt = DateTime.UtcNow.AddHours(8);

        _repoMock.Setup(r => r.BuscarPorLoginAsync("admin", default))
                 .ReturnsAsync(usuario);
        _passwordHasherMock.Setup(p => p.Verificar("camaradinha@123", usuario.SenhaHash))
                           .Returns(true);
        _tokenServiceMock.Setup(t => t.GerarToken(usuario))
                         .Returns(("jwt.token.valido", expiresAt));

        var svc = CriarService();
        var result = await svc.Authenticate(new LoginCommand("admin", "camaradinha@123"));

        result.Sucesso.Should().BeTrue();
        result.Token.Should().NotBeNullOrWhiteSpace();
        result.ExpiresAt.Should().NotBeNull().And.BeAfter(DateTime.UtcNow);
        result.Requer2FA.Should().BeFalse();
        result.Erro.Should().BeNull();
    }

    [Fact]
    public async Task Authenticate_CredenciaisValidas_GerarTokenChamadoExatamenteUmaVez()
    {
        var usuario = CriarAdmin();

        _repoMock.Setup(r => r.BuscarPorLoginAsync("admin", default)).ReturnsAsync(usuario);
        _passwordHasherMock.Setup(p => p.Verificar(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        _tokenServiceMock.Setup(t => t.GerarToken(usuario)).Returns(("tok", DateTime.UtcNow.AddHours(1)));

        var svc = CriarService();

        await svc.Authenticate(new LoginCommand("admin", "camaradinha@123"));

        _tokenServiceMock.Verify(t => t.GerarToken(usuario), Times.Once);
    }

    [Fact]
    public async Task Authenticate_LoginInexistente_RetornaFalhaComMensagemGenerica()
    {
        _repoMock.Setup(r => r.BuscarPorLoginAsync("naoexiste", default))
                 .ReturnsAsync((Usuario?)null);
        _passwordHasherMock.Setup(p => p.Verificar(It.IsAny<string>(), It.IsAny<string>()))
                           .Returns(false);

        var svc = CriarService();
        var result = await svc.Authenticate(new LoginCommand("naoexiste", "qualquer"));

        result.Sucesso.Should().BeFalse();
        result.Token.Should().BeNull();
        result.Erro.Should().Be("Credenciais inválidas.");
    }

    [Fact]
    public async Task Authenticate_SenhaIncorreta_RetornaMesmaMensagemQueLoginInexistente()
    {
        var usuario = CriarAdmin();

        _repoMock.Setup(r => r.BuscarPorLoginAsync("admin", default)).ReturnsAsync(usuario);
        _passwordHasherMock.Setup(p => p.Verificar("senhaErrada", usuario.SenhaHash)).Returns(false);

        var svc = CriarService();
        var resultSenhaErrada = await svc.Authenticate(new LoginCommand("admin", "senhaErrada"));

        // simular inexistente
        _repoMock.Setup(r => r.BuscarPorLoginAsync("naoexiste", default)).ReturnsAsync((Usuario?)null);
        _passwordHasherMock.Setup(p => p.Verificar(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
        var resultLoginInexistente = await svc.Authenticate(new LoginCommand("naoexiste", "qualquer"));

        resultSenhaErrada.Erro.Should().Be(resultLoginInexistente.Erro);
        resultSenhaErrada.Sucesso.Should().BeFalse();
        resultSenhaErrada.Token.Should().BeNull();
    }

    [Fact]
    public async Task Authenticate_UsuarioInativo_RetornaFalhaTokenNulo()
    {
        var usuarioInativo = CriarAdmin(ativo: false);

        _repoMock.Setup(r => r.BuscarPorLoginAsync("admin", default)).ReturnsAsync(usuarioInativo);
        _passwordHasherMock.Setup(p => p.Verificar(It.IsAny<string>(), It.IsAny<string>()))
                           .Returns(true);

        var svc = CriarService();
        var result = await svc.Authenticate(new LoginCommand("admin", "camaradinha@123"));

        result.Sucesso.Should().BeFalse();
        result.Token.Should().BeNull();
    }

    [Fact]
    public async Task Authenticate_UsuarioComRequer2FA_RetornaRequer2FATrueTokenNulo()
    {
        var usuarioCom2FA = CriarAdmin(requer2FA: true);

        _repoMock.Setup(r => r.BuscarPorLoginAsync("admin", default)).ReturnsAsync(usuarioCom2FA);
        _passwordHasherMock.Setup(p => p.Verificar("camaradinha@123", usuarioCom2FA.SenhaHash))
                           .Returns(true);

        var svc = CriarService();
        var result = await svc.Authenticate(new LoginCommand("admin", "camaradinha@123"));

        result.Requer2FA.Should().BeTrue();
        result.Token.Should().BeNull();
        result.Sucesso.Should().BeFalse();
    }
}
