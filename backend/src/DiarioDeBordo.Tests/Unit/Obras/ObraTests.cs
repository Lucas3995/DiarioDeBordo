using DiarioDeBordo.Domain.Obras;
using FluentAssertions;

namespace DiarioDeBordo.Tests.Unit.Obras;

/// <summary>
/// Testes unitários para a entidade Obra.
/// Seguem ciclo TDD (Red-Green-Refactor) e padrão AAA.
/// Verificam invariantes do domínio sem dependências externas.
/// </summary>
public sealed class ObraTests
{
    // --------------- Criação válida ---------------

    [Fact]
    public void Criar_ComDadosValidos_DeveCriarObra()
    {
        // Arrange
        var data = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);

        // Act
        var obra = new Obra("One Piece", TipoObra.Manga, 1100, data, ordemPreferencia: 1);

        // Assert
        obra.Nome.Should().Be("One Piece");
        obra.Tipo.Should().Be(TipoObra.Manga);
        obra.PosicaoAtual.Should().Be(1100);
        obra.DataUltimaAtualizacaoPosicao.Should().Be(data);
        obra.OrdemPreferencia.Should().Be(1);
        obra.ProximaInfoTipo.Should().BeNull();
        obra.DiasAteProximaParte.Should().BeNull();
        obra.PartesJaPublicadas.Should().BeNull();
        obra.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Criar_DeveTerIdUnico()
    {
        // Arrange & Act
        var data = DateTime.UtcNow;
        var obra1 = new Obra("Obra A", TipoObra.Anime, 10, data, 1);
        var obra2 = new Obra("Obra B", TipoObra.Livro, 5, data, 2);

        // Assert
        obra1.Id.Should().NotBe(obra2.Id);
    }

    // --------------- Invariante: nome ---------------

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_ComNomeVazioOuNulo_DeveLancarExcecao(string nome)
    {
        // Arrange & Act
        var act = () => new Obra(nome, TipoObra.Manga, 1, DateTime.UtcNow, 0);

        // Assert
        act.Should().Throw<ArgumentException>().WithParameterName("nome");
    }

    [Fact]
    public void Criar_ComNomeMuitoLongo_DeveLancarExcecao()
    {
        // Arrange
        var nomeLongo = new string('x', 301);

        // Act
        var act = () => new Obra(nomeLongo, TipoObra.Manga, 1, DateTime.UtcNow, 0);

        // Assert
        act.Should().Throw<ArgumentException>().WithParameterName("nome");
    }

    // --------------- Invariante: posição ---------------

    [Fact]
    public void Criar_ComPosicaoNegativa_DeveLancarExcecao()
    {
        // Arrange & Act
        var act = () => new Obra("Obra X", TipoObra.Manga, -1, DateTime.UtcNow, 0);

        // Assert
        act.Should().Throw<ArgumentException>().WithParameterName("posicaoAtual");
    }

    [Fact]
    public void Criar_ComPosicaoZero_DevePermitir()
    {
        // Arrange & Act
        var act = () => new Obra("Nova Obra", TipoObra.Serie, 0, DateTime.UtcNow, 0);

        // Assert
        act.Should().NotThrow();
    }

    // --------------- Invariante: ordem de preferência ---------------

    [Fact]
    public void Criar_ComOrdemNegativa_DeveLancarExcecao()
    {
        // Arrange & Act
        var act = () => new Obra("Obra X", TipoObra.Manga, 1, DateTime.UtcNow, ordemPreferencia: -1);

        // Assert
        act.Should().Throw<ArgumentException>().WithParameterName("ordemPreferencia");
    }

    [Fact]
    public void Criar_ComOrdemZero_DevePermitir()
    {
        // Arrange & Act
        var act = () => new Obra("Obra Top", TipoObra.Manga, 1, DateTime.UtcNow, ordemPreferencia: 0);

        // Assert
        act.Should().NotThrow();
    }

    // --------------- ProximaInfo: dias até próxima ---------------

    [Fact]
    public void DefinirDiasAteProxima_ComValorValido_DeveSetarCampos()
    {
        // Arrange
        var obra = new Obra("Vinland Saga", TipoObra.Manga, 200, DateTime.UtcNow, 1);

        // Act
        obra.DefinirDiasAteProxima(7);

        // Assert
        obra.ProximaInfoTipo.Should().Be(ProximaInfoTipo.DiasAteProxima);
        obra.DiasAteProximaParte.Should().Be(7);
        obra.PartesJaPublicadas.Should().BeNull();
    }

    [Fact]
    public void DefinirDiasAteProxima_ComValorNegativo_DeveLancarExcecao()
    {
        // Arrange
        var obra = new Obra("Obra X", TipoObra.Manga, 1, DateTime.UtcNow, 0);

        // Act
        var act = () => obra.DefinirDiasAteProxima(-1);

        // Assert
        act.Should().Throw<ArgumentException>().WithParameterName("dias");
    }

    // --------------- ProximaInfo: partes já publicadas ---------------

    [Fact]
    public void DefinirPartesJaPublicadas_ComValorValido_DeveSetarCampos()
    {
        // Arrange
        var obra = new Obra("Tower of God", TipoObra.Manhwa, 600, DateTime.UtcNow, 2);

        // Act
        obra.DefinirPartesJaPublicadas(3);

        // Assert
        obra.ProximaInfoTipo.Should().Be(ProximaInfoTipo.PartesJaPublicadas);
        obra.PartesJaPublicadas.Should().Be(3);
        obra.DiasAteProximaParte.Should().BeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void DefinirPartesJaPublicadas_ComValorNaoPositivo_DeveLancarExcecao(int quantidade)
    {
        // Arrange
        var obra = new Obra("Obra X", TipoObra.Manhwa, 1, DateTime.UtcNow, 0);

        // Act
        var act = () => obra.DefinirPartesJaPublicadas(quantidade);

        // Assert
        act.Should().Throw<ArgumentException>().WithParameterName("quantidade");
    }

    // --------------- LimparProximaInfo ---------------

    [Fact]
    public void LimparProximaInfo_ApósDefinir_DeveLimparTodosOsCampos()
    {
        // Arrange
        var obra = new Obra("Berserk", TipoObra.Manga, 370, DateTime.UtcNow, 0);
        obra.DefinirDiasAteProxima(14);

        // Act
        obra.LimparProximaInfo();

        // Assert
        obra.ProximaInfoTipo.Should().BeNull();
        obra.DiasAteProximaParte.Should().BeNull();
        obra.PartesJaPublicadas.Should().BeNull();
    }

    // --------------- Substituição de ProximaInfo ---------------

    [Fact]
    public void DefinirPartesJaPublicadas_ApósDiasAteProxima_DeveSobrescrever()
    {
        // Arrange
        var obra = new Obra("Solo Leveling", TipoObra.Manhwa, 179, DateTime.UtcNow, 1);
        obra.DefinirDiasAteProxima(7);

        // Act
        obra.DefinirPartesJaPublicadas(2);

        // Assert
        obra.ProximaInfoTipo.Should().Be(ProximaInfoTipo.PartesJaPublicadas);
        obra.PartesJaPublicadas.Should().Be(2);
        obra.DiasAteProximaParte.Should().BeNull();
    }

    // --------------- AtualizarPosicao (relatório item 1) ---------------

    [Fact]
    public void AtualizarPosicao_ComNovaPosicaoEData_DeveAtualizarPosicaoEData()
    {
        // Arrange
        var dataInicial = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        var obra = new Obra("One Piece", TipoObra.Manga, 1100, dataInicial, 1);
        var novaData = new DateTime(2026, 2, 15, 12, 0, 0, DateTimeKind.Utc);

        // Act
        obra.AtualizarPosicao(1115, novaData);

        // Assert
        obra.PosicaoAtual.Should().Be(1115);
        obra.DataUltimaAtualizacaoPosicao.Should().Be(novaData);
    }

    [Fact]
    public void AtualizarPosicao_ComDataNula_DeveUsarUtcNow()
    {
        // Arrange
        var dataInicial = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        var obra = new Obra("Obra X", TipoObra.Manga, 10, dataInicial, 0);
        var antes = DateTime.UtcNow;

        // Act
        obra.AtualizarPosicao(20, dataUltimaAtualizacao: null);

        // Assert
        obra.PosicaoAtual.Should().Be(20);
        obra.DataUltimaAtualizacaoPosicao.Should().BeOnOrAfter(antes).And.BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void AtualizarPosicao_ComPosicaoNegativa_DeveLancarExcecao()
    {
        // Arrange
        var obra = new Obra("Obra X", TipoObra.Manga, 10, DateTime.UtcNow, 0);

        // Act
        var act = () => obra.AtualizarPosicao(-1, DateTime.UtcNow);

        // Assert
        act.Should().Throw<ArgumentException>().WithParameterName("posicaoAtual");
    }

    [Fact]
    public void AtualizarPosicao_ComPosicaoZero_DevePermitir()
    {
        // Arrange
        var obra = new Obra("Obra Reiniciada", TipoObra.Livro, 5, DateTime.UtcNow, 0);

        // Act
        obra.AtualizarPosicao(0, DateTime.UtcNow);

        // Assert
        obra.PosicaoAtual.Should().Be(0);
    }
}
