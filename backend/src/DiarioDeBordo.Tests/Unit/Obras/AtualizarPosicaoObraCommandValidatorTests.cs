using DiarioDeBordo.Application.Obras.AtualizarPosicao;
using DiarioDeBordo.Domain.Obras;
using FluentAssertions;

namespace DiarioDeBordo.Tests.Unit.Obras;

/// <summary>
/// Testes unitários para AtualizarPosicaoObraCommandValidator.
/// Relatório item 5: validação do command.
/// </summary>
public sealed class AtualizarPosicaoObraCommandValidatorTests
{
    private readonly AtualizarPosicaoObraCommandValidator _validator = new();

    [Fact]
    public void Validar_SemIdNemNome_DeveFalhar()
    {
        var command = new AtualizarPosicaoObraCommand(IdObra: null, NomeObra: null, NovaPosicao: 10, DataUltimaAtualizacao: null, CriarSeNaoExistir: false);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Validar_ComIdObra_DevePassar()
    {
        var command = new AtualizarPosicaoObraCommand(IdObra: Guid.NewGuid(), NomeObra: null, NovaPosicao: 10, DataUltimaAtualizacao: null, CriarSeNaoExistir: false);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validar_ComNomeObra_DevePassar()
    {
        var command = new AtualizarPosicaoObraCommand(IdObra: null, NomeObra: "Obra X", NovaPosicao: 5, DataUltimaAtualizacao: null, CriarSeNaoExistir: false);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validar_NovaPosicaoNegativa_DeveFalhar()
    {
        var command = new AtualizarPosicaoObraCommand(IdObra: Guid.NewGuid(), NomeObra: null, NovaPosicao: -1, DataUltimaAtualizacao: null, CriarSeNaoExistir: false);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AtualizarPosicaoObraCommand.NovaPosicao));
    }

    [Fact]
    public void Validar_CriarSeNaoExistirSemNomeParaCriar_DeveFalhar()
    {
        var command = new AtualizarPosicaoObraCommand(
            IdObra: null, NomeObra: "X", NovaPosicao: 1, DataUltimaAtualizacao: null, CriarSeNaoExistir: true,
            NomeParaCriar: null, TipoParaCriar: null, OrdemPreferenciaParaCriar: null);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validar_CriarSeNaoExistirComDadosCompletos_DevePassar()
    {
        var command = new AtualizarPosicaoObraCommand(
            IdObra: null, NomeObra: "Nova", NovaPosicao: 1, DataUltimaAtualizacao: null, CriarSeNaoExistir: true,
            NomeParaCriar: "Nova", TipoParaCriar: TipoObra.Manga, OrdemPreferenciaParaCriar: 0);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }
}
