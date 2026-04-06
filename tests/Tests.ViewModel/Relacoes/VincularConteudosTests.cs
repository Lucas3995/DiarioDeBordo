namespace DiarioDeBordo.Tests.ViewModel.Relacoes;

/// <summary>
/// Valida a lógica de habilitação do botão "Vincular conteúdos" (PodeVincular).
/// Simula o SelectedItem TwoWay binding e o AsyncPopulator sem rodar a UI.
/// </summary>
public class VincularConteudosTests
{
    private readonly IMediator _mediator;
    private readonly ConteudoDetalheViewModel _vm;

    // Dados fixos de teste
    private readonly TipoRelacaoDto _tipoExistente =
        new(Guid.NewGuid(), "Sequência", "Sequência de", false);

    private readonly ConteudoResumoDto _conteudoAlvo =
        new(Guid.NewGuid(), "Livro X", "Livro", "Standalone",
            DateTimeOffset.Now, null, null, null);

    public VincularConteudosTests()
    {
        _mediator = Substitute.For<IMediator>();
        var dialogService = Substitute.For<IDialogService>();
        _vm = new ConteudoDetalheViewModel(_mediator, Guid.NewGuid(), dialogService);

        // Setup: PreCarregarTiposRelacao retorna [_tipoExistente]
        _mediator.Send(Arg.Any<BuscarTiposRelacaoQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<TipoRelacaoDto>>([_tipoExistente]));

        // Setup: PopularSugestoesConteudo retorna [_conteudoAlvo]
        _mediator.Send(Arg.Any<BuscarConteudosQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<ConteudoResumoDto>>([_conteudoAlvo]));
    }

    // ──────────────────────────────────────────────────────────────────────
    // Estado inicial
    // ──────────────────────────────────────────────────────────────────────

    [Fact]
    public void PodeVincular_EstadoInicial_DeveFalso()
    {
        Assert.False(_vm.PodeVincular);
    }

    // ──────────────────────────────────────────────────────────────────────
    // Fluxo principal: tipo EXISTENTE
    // ──────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task PodeVincular_SoConteudoSelecionado_SemTipo_DeveFalso()
    {
        await _vm.PopularSugestoesConteudoAsync("Livro");

        _vm.ConteudoAlvoSelecionadoItem = "Livro X"; // simula SelectedItem TwoWay binding

        Assert.NotNull(_vm.ConteudoAlvoSelecionado);
        Assert.False(_vm.PodeVincular);
    }

    [Fact]
    public async Task PodeVincular_SoTipoSelecionado_SemConteudo_DeveFalso()
    {
        await _vm.PreCarregarTiposRelacaoAsync();

        _vm.TipoSelecionadoItem = "Sequência"; // simula SelectedItem TwoWay binding

        Assert.NotNull(_vm.TipoRelacaoSelecionado);
        Assert.False(_vm.PodeVincular);
    }

    [Fact]
    public async Task PodeVincular_ConteudoETipoExistentesSelecionados_DeveSerTrue()
    {
        // Arrange: simula OnOpened + AsyncPopulator
        await _vm.PreCarregarTiposRelacaoAsync();
        await _vm.PopularSugestoesConteudoAsync("Livro");

        // Act: simula usuário selecionando conteúdo (SelectedItem TwoWay)
        _vm.ConteudoAlvoSelecionadoItem = "Livro X";

        Assert.NotNull(_vm.ConteudoAlvoSelecionado); // chip deve aparecer
        Assert.False(_vm.PodeVincular);              // tipo ainda não selecionado

        // Act: simula usuário selecionando tipo (SelectedItem TwoWay)
        _vm.TipoSelecionadoItem = "Sequência";

        Assert.NotNull(_vm.TipoRelacaoSelecionado);
        Assert.True(_vm.PodeVincular); // DEVE ser true agora!
    }

    // ──────────────────────────────────────────────────────────────────────
    // Fluxo novo tipo: nome digitado que não existe no banco
    // ──────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task PodeVincular_TipoNovaSemNomeInverso_DeveFalso()
    {
        // Sem tipos no banco
        _mediator.Send(Arg.Any<BuscarTiposRelacaoQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<TipoRelacaoDto>>([]));

        await _vm.PreCarregarTiposRelacaoAsync();
        await _vm.PopularSugestoesConteudoAsync("Livro");

        _vm.ConteudoAlvoSelecionadoItem = "Livro X";
        _vm.SelecionarOuCriarTipoRelacao("TipoNovo"); // nome não existe → CriandoNovoTipo=true

        Assert.True(_vm.CriandoNovoTipo);
        Assert.Null(_vm.TipoRelacaoSelecionado);
        Assert.False(_vm.PodeVincular);
    }

    [Fact]
    public async Task PodeVincular_TipoNovoComNomeInverso_DeveSerTrue()
    {
        _mediator.Send(Arg.Any<BuscarTiposRelacaoQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<TipoRelacaoDto>>([]));

        await _vm.PreCarregarTiposRelacaoAsync();
        await _vm.PopularSugestoesConteudoAsync("Livro");

        _vm.ConteudoAlvoSelecionadoItem = "Livro X";
        _vm.SelecionarOuCriarTipoRelacao("TipoNovo");
        _vm.NomeInversoNovoTipo = "Inverso do TipoNovo";

        Assert.True(_vm.PodeVincular);
    }

    // ──────────────────────────────────────────────────────────────────────
    // Cancelar limpa estado
    // ──────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task PodeVincular_AposCancelar_DeveSerFalso()
    {
        await _vm.PreCarregarTiposRelacaoAsync();
        await _vm.PopularSugestoesConteudoAsync("Livro");

        _vm.ConteudoAlvoSelecionadoItem = "Livro X";
        _vm.TipoSelecionadoItem = "Sequência";
        Assert.True(_vm.PodeVincular);

        _vm.CancelarFormularioRelacaoCommand.Execute(null);

        Assert.Null(_vm.ConteudoAlvoSelecionado);
        Assert.Null(_vm.TipoRelacaoSelecionado);
        Assert.False(_vm.PodeVincular);
    }
}
