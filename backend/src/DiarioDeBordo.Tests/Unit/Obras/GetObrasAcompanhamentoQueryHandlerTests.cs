using DiarioDeBordo.Application.Obras.Listar;
using DiarioDeBordo.Domain.Obras;
using FluentAssertions;
using Moq;

namespace DiarioDeBordo.Tests.Unit.Obras;

/// <summary>
/// Testes unitários para GetObrasAcompanhamentoQueryHandler.
/// Isolam o handler via Mock de IObraLeituraRepository.
/// Cobrem: paginação, ordenação, projeção de campos e mapeamento de ProximaInfo.
/// Padrão AAA; sem dependências de banco ou EF Core.
/// </summary>
public sealed class GetObrasAcompanhamentoQueryHandlerTests
{
    private readonly Mock<IObraLeituraRepository> _repositorioMock;
    private readonly GetObrasAcompanhamentoQueryHandler _handler;

    public GetObrasAcompanhamentoQueryHandlerTests()
    {
        _repositorioMock = new Mock<IObraLeituraRepository>();
        _handler = new GetObrasAcompanhamentoQueryHandler(_repositorioMock.Object);
    }

    // --- Delegação ao repositório ---

    [Fact]
    public async Task Handle_DevePassarPageIndexEPageSizeParaRepositorio()
    {
        // Arrange
        ConfigurarRepositorio([], totalCount: 0);
        var query = new GetObrasAcompanhamentoQuery(PageIndex: 2, PageSize: 25);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositorioMock.Verify(
            r => r.ListarPaginadoAsync(2, 25, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // --- TotalCount ---

    [Fact]
    public async Task Handle_DeveRetornarTotalCountDoRepositorio()
    {
        // Arrange
        ConfigurarRepositorio([], totalCount: 42);
        var query = new GetObrasAcompanhamentoQuery(0, 10);

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.TotalCount.Should().Be(42);
    }

    // --- Paginação ---

    [Fact]
    public async Task Handle_DeveRetornarQuantidadeCorretaDeItens()
    {
        // Arrange
        var obras = CriarObras(5);
        ConfigurarRepositorio(obras, totalCount: 20);
        var query = new GetObrasAcompanhamentoQuery(PageIndex: 0, PageSize: 10);

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.Items.Should().HaveCount(5);
    }

    // --- Projeção de campos ---

    [Fact]
    public async Task Handle_DeveProjetarCamposCorretamente()
    {
        // Arrange
        var data = new DateTime(2024, 6, 15, 8, 0, 0, DateTimeKind.Utc);
        var obra = new Obra("One Piece", TipoObra.Manga, 1100, data, ordemPreferencia: 1);
        ConfigurarRepositorio([obra], totalCount: 1);
        var query = new GetObrasAcompanhamentoQuery(0, 10);

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var item = resultado.Items.Single();
        item.Nome.Should().Be("One Piece");
        item.Tipo.Should().Be("manga");
        item.PosicaoAtual.Should().Be(1100);
        item.UltimaAtualizacaoPosicao.Should().Be(data);
        item.OrdemPreferencia.Should().Be(1);
        item.Id.Should().Be(obra.Id.ToString());
        item.ProximaInfo.Should().BeNull();
    }

    [Fact]
    public async Task Handle_DeveMaperarTipoEmMinusculas()
    {
        // Arrange
        var obras = new Obra[]
        {
            new("Manga X", TipoObra.Manga, 1, DateTime.UtcNow, 1),
            new("Anime Y", TipoObra.Anime, 10, DateTime.UtcNow, 2),
            new("Serie Z", TipoObra.Serie, 3, DateTime.UtcNow, 3),
            new("Manhwa W", TipoObra.Manhwa, 50, DateTime.UtcNow, 4),
            new("Livro V", TipoObra.Livro, 20, DateTime.UtcNow, 5),
        };
        ConfigurarRepositorio(obras, totalCount: obras.Length);
        var query = new GetObrasAcompanhamentoQuery(0, 10);

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.Items.Select(i => i.Tipo)
            .Should().BeEquivalentTo(["manga", "anime", "serie", "manhwa", "livro"]);
    }

    // --- ProximaInfo: dias até próxima ---

    [Fact]
    public async Task Handle_ComDiasAteProxima_DeveMapearProximaInfoCorretamente()
    {
        // Arrange
        var obra = new Obra("Vinland Saga", TipoObra.Manga, 200, DateTime.UtcNow, 1);
        obra.DefinirDiasAteProxima(7);
        ConfigurarRepositorio([obra], totalCount: 1);
        var query = new GetObrasAcompanhamentoQuery(0, 10);

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var proxima = resultado.Items.Single().ProximaInfo;
        proxima.Should().NotBeNull();
        proxima!.Tipo.Should().Be("dias_ate_proxima");
        proxima.Dias.Should().Be(7);
        proxima.Quantidade.Should().BeNull();
    }

    // --- ProximaInfo: partes já publicadas ---

    [Fact]
    public async Task Handle_ComPartesJaPublicadas_DeveMapearProximaInfoCorretamente()
    {
        // Arrange
        var obra = new Obra("Tower of God", TipoObra.Manhwa, 600, DateTime.UtcNow, 2);
        obra.DefinirPartesJaPublicadas(3);
        ConfigurarRepositorio([obra], totalCount: 1);
        var query = new GetObrasAcompanhamentoQuery(0, 10);

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var proxima = resultado.Items.Single().ProximaInfo;
        proxima.Should().NotBeNull();
        proxima!.Tipo.Should().Be("partes_ja_publicadas");
        proxima.Quantidade.Should().Be(3);
        proxima.Dias.Should().BeNull();
    }

    // --- Lista vazia ---

    [Fact]
    public async Task Handle_ComListaVazia_DeveRetornarItensVaziosETotalZero()
    {
        // Arrange
        ConfigurarRepositorio([], totalCount: 0);
        var query = new GetObrasAcompanhamentoQuery(0, 10);

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.Items.Should().BeEmpty();
        resultado.TotalCount.Should().Be(0);
    }

    // --- Validação do validator ---

    [Theory]
    [InlineData(-1, 10)]
    [InlineData(0, 5)]
    [InlineData(0, 0)]
    [InlineData(0, 15)]
    [InlineData(0, 200)]
    public void Validar_ComParametrosInvalidos_DeveFalhar(int pageIndex, int pageSize)
    {
        // Arrange
        var validator = new GetObrasAcompanhamentoQueryValidator();
        var query = new GetObrasAcompanhamentoQuery(pageIndex, pageSize);

        // Act
        var resultado = validator.Validate(query);

        // Assert
        resultado.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(0, 25)]
    [InlineData(1, 50)]
    [InlineData(5, 100)]
    public void Validar_ComParametrosValidos_DevePassar(int pageIndex, int pageSize)
    {
        // Arrange
        var validator = new GetObrasAcompanhamentoQueryValidator();
        var query = new GetObrasAcompanhamentoQuery(pageIndex, pageSize);

        // Act
        var resultado = validator.Validate(query);

        // Assert
        resultado.IsValid.Should().BeTrue();
    }

    // --- Helpers ---

    private void ConfigurarRepositorio(IReadOnlyList<Obra> obras, int totalCount)
    {
        _repositorioMock
            .Setup(r => r.ListarPaginadoAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((obras, totalCount));
    }

    private static Obra[] CriarObras(int quantidade)
    {
        return Enumerable.Range(1, quantidade)
            .Select(i => new Obra($"Obra {i}", TipoObra.Manga, i * 10, DateTime.UtcNow, ordemPreferencia: i))
            .ToArray();
    }
}
