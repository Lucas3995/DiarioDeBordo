using DiarioDeBordo.Application.Auth.Login;
using FluentAssertions;
using Xunit;

namespace DiarioDeBordo.Tests.Unit.Auth;

public sealed class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    // ───────────────────────── Login ─────────────────────────

    [Fact]
    public void Validar_LoginVazio_Invalido()
    {
        var result = _validator.Validate(new LoginCommand("", "senha123"));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(LoginCommand.Login));
    }

    [Fact]
    public void Validar_LoginComEspacosApenas_Invalido()
    {
        var result = _validator.Validate(new LoginCommand("   ", "senha123"));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validar_LoginComMaisDe100Chars_Invalido()
    {
        var loginLongo = new string('a', 101);
        var result = _validator.Validate(new LoginCommand(loginLongo, "senha123"));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(LoginCommand.Login));
    }

    [Fact]
    public void Validar_LoginComExatamente100Chars_Valido()
    {
        var login100 = new string('a', 100);
        var result = _validator.Validate(new LoginCommand(login100, "senha123"));
        result.Errors.Should().NotContain(e => e.PropertyName == nameof(LoginCommand.Login));
    }

    // ───────────────────────── Senha ─────────────────────────

    [Fact]
    public void Validar_SenhaVazia_Invalido()
    {
        var result = _validator.Validate(new LoginCommand("admin", ""));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(LoginCommand.Senha));
    }

    [Fact]
    public void Validar_SenhaComMaisDe200Chars_Invalido()
    {
        var senhaLonga = new string('a', 201);
        var result = _validator.Validate(new LoginCommand("admin", senhaLonga));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(LoginCommand.Senha));
    }

    [Fact]
    public void Validar_SenhaComExatamente200Chars_Valido()
    {
        var senha200 = new string('a', 200);
        var result = _validator.Validate(new LoginCommand("admin", senha200));
        result.Errors.Should().NotContain(e => e.PropertyName == nameof(LoginCommand.Senha));
    }

    // ───────────────────────── Caso válido ─────────────────────────

    [Fact]
    public void Validar_LoginESenhaValidos_Valido()
    {
        var result = _validator.Validate(new LoginCommand("admin", "camaradinha@123"));
        result.IsValid.Should().BeTrue();
    }
}
