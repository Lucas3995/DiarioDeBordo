using DiarioDeBordo.Application.Obras.ObterPorIdOuNome;
using FluentAssertions;

namespace DiarioDeBordo.Tests.Unit.Obras;

/// <summary>
/// Testes unitários para GetObraPorIdOuNomeQueryValidator.
/// Relatório item 6: pelo menos um de Id ou Nome obrigatório.
/// </summary>
public sealed class GetObraPorIdOuNomeQueryValidatorTests
{
    private readonly GetObraPorIdOuNomeQueryValidator _validator = new();

    [Fact]
    public void Validar_SemIdNemNome_DeveFalhar()
    {
        var query = new GetObraPorIdOuNomeQuery(Id: null, Nome: null);
        var result = _validator.Validate(query);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validar_ComId_DevePassar()
    {
        var query = new GetObraPorIdOuNomeQuery(Id: Guid.NewGuid(), Nome: null);
        var result = _validator.Validate(query);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validar_ComNome_DevePassar()
    {
        var query = new GetObraPorIdOuNomeQuery(Id: null, Nome: "Obra X");
        var result = _validator.Validate(query);
        result.IsValid.Should().BeTrue();
    }
}
