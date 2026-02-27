using DiarioDeBordo.Application.Echo;
using FluentAssertions;

namespace DiarioDeBordo.Tests.Unit;

/// <summary>
/// Testes unitários para EchoCommandValidator.
/// Verifica que a validação FluentValidation funciona corretamente.
/// Padrão AAA; sem dependências externas.
/// </summary>
public sealed class EchoCommandValidatorTests
{
    private readonly EchoCommandValidator _validator = new();

    [Fact]
    public void Validate_ComMensagemValida_DevePassar()
    {
        // Arrange
        var command = new EchoCommand("Olá, mundo!");

        // Act
        var resultado = _validator.Validate(command);

        // Assert
        resultado.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ComMensagemVazia_DeveFalhar()
    {
        // Arrange
        var command = new EchoCommand(string.Empty);

        // Act
        var resultado = _validator.Validate(command);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().ContainSingle(e =>
            e.PropertyName == nameof(EchoCommand.Mensagem));
    }

    [Fact]
    public void Validate_ComMensagemMuitoLonga_DeveFalhar()
    {
        // Arrange
        var mensagemLonga = new string('x', 257);
        var command = new EchoCommand(mensagemLonga);

        // Act
        var resultado = _validator.Validate(command);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().ContainSingle(e =>
            e.PropertyName == nameof(EchoCommand.Mensagem));
    }

    [Fact]
    public void Validate_ComMensagemNoLimiteMaximo_DevePassar()
    {
        // Arrange
        var mensagem256 = new string('x', 256);
        var command = new EchoCommand(mensagem256);

        // Act
        var resultado = _validator.Validate(command);

        // Assert
        resultado.IsValid.Should().BeTrue();
    }
}
