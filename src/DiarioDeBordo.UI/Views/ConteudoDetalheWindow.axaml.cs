using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using DiarioDeBordo.UI.ViewModels;

#pragma warning disable CA2007 // UI View: event handlers must stay on UI thread — no ConfigureAwait

namespace DiarioDeBordo.UI.Views;

/// <summary>
/// Janela modal de detalhe e edição de conteúdo.
/// Exibe 4 seções expandíveis (Identificação, Avaliação, Organização, Histórico).
/// </summary>
public partial class ConteudoDetalheWindow : Window
{
    // Set to true before any programmatic Close() call so OnClosing skips the dirty check.
    // This prevents Salvar/Cancelar/Excluir from re-triggering the discard dialog.
    private bool _permitirFechamento;

    /// <summary>
    /// Result set by FecharJanela() and read by DialogService after window.Closed fires.
    /// Needed because Show() (non-modal) doesn't propagate Close(result) like ShowDialog does.
    /// </summary>
    public bool? WindowResult { get; private set; }

    public ConteudoDetalheWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Closes the window programmatically, bypassing the dirty-check in OnClosing.
    /// All ViewModel commands must use this method instead of calling Close() directly.
    /// </summary>
    public void FecharJanela(bool? result)
    {
        _permitirFechamento = true;
        WindowResult = result;
        Close(result);
    }

    protected override async void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        // Set owner reference on ViewModel for Close() calls
        if (DataContext is ConteudoDetalheViewModel vm)
        {
            vm.Owner = this;
            // Trigger loading after window is shown — must complete before PreCarregar
            // to avoid concurrent DbContext usage (both share the same context via root DI).
            await vm.CarregarCommand.ExecuteAsync(null);

            // Pre-load relation types — small dataset, loaded once; refreshed after Salvar
            await vm.PreCarregarTiposRelacaoAsync();

            // Wire AutoCompleteBox async populators — AsyncPopulator is the correct Avalonia 11 API
            // for async data; Populating+Cancel+Populated is error-prone and the old approach
            // (Populating only) never set ItemsSource so the dropdown was always empty.
            if (this.FindControl<AutoCompleteBox>("CategoriaAutoComplete") is AutoCompleteBox categoriaAc)
            {
                categoriaAc.AsyncPopulator = async (text, ct) =>
                {
                    await vm.PopularSugestoesCategoriasAsync(text ?? string.Empty);
                    return vm.SugestoesCategoria.Select(c => c.Nome).Cast<object>();
                };

                categoriaAc.SelectionChanged += async (_, args) =>
                {
                    if (args.AddedItems.Count > 0 && args.AddedItems[0] is string nome)
                    {
                        await vm.SelecionarCategoriaAsync(nome);
                        categoriaAc.Text = string.Empty;
                        categoriaAc.SelectedItem = null;
                    }
                };

                // Enter key: add typed text as new or existing category
                categoriaAc.KeyDown += async (_, args) =>
                {
                    if (args.Key == Avalonia.Input.Key.Return && !string.IsNullOrWhiteSpace(categoriaAc.Text))
                    {
                        var text = categoriaAc.Text!;
                        categoriaAc.Text = string.Empty;
                        args.Handled = true;
                        await vm.SelecionarCategoriaAsync(text);
                    }
                };
            }

            if (this.FindControl<AutoCompleteBox>("ConteudoAlvoAutoComplete") is AutoCompleteBox conteudoAc)
            {
                conteudoAc.AsyncPopulator = async (text, ct) =>
                {
                    await vm.PopularSugestoesConteudoAsync(text ?? string.Empty);
                    return vm.SugestoesConteudo.Select(c => c.Titulo).Cast<object>();
                };

                conteudoAc.SelectionChanged += (_, args) =>
                {
                    if (args.AddedItems.Count > 0 && args.AddedItems[0] is string titulo)
                    {
                        vm.ConteudoAlvoSelecionado = vm.SugestoesConteudo
                            .FirstOrDefault(c => c.Titulo == titulo);
                        // Clear text — the selected content title is shown via chip/label, not the input
                    }
                };
            }

            // TipoRelacao uses AsyncPopulator (same pattern as Categoria/ConteudoAlvo) because
            // SelectionChanged only fires reliably in Avalonia 11 when the populator manages the list.
            // Data is already pre-loaded in NomesTiposRelacao — no extra DB call needed.
            if (this.FindControl<AutoCompleteBox>("TipoRelacaoAutoComplete") is AutoCompleteBox tipoAc)
            {
                tipoAc.AsyncPopulator = (text, ct) =>
                {
                    var filtro = text ?? string.Empty;
                    IEnumerable<object> items = filtro.Length == 0
                        ? vm.NomesTiposRelacao.Cast<object>()
                        : vm.NomesTiposRelacao
                            .Where(n => n.Contains(filtro, StringComparison.OrdinalIgnoreCase))
                            .Cast<object>();
                    return Task.FromResult(items);
                };

                // Selection via click is handled by SelectedItem TwoWay binding in AXAML.
                // User pressed Enter to confirm a new or existing type name
                tipoAc.KeyDown += (_, args) =>
                {
                    if (args.Key == Avalonia.Input.Key.Return && !string.IsNullOrWhiteSpace(tipoAc.Text))
                    {
                        var text = tipoAc.Text!;
                        args.Handled = true;
                        vm.SelecionarOuCriarTipoRelacao(text);
                    }
                };
            }
        }
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        // Programmatic close (Salvar/Cancelar/Excluir commands) — let it through.
        if (_permitirFechamento)
        {
            base.OnClosing(e);
            return;
        }

        // User-initiated close (X button): ask for discard confirmation when dirty.
        if (DataContext is ConteudoDetalheViewModel vm && vm.IsDirty)
        {
            e.Cancel = true;
            // Trigger cancel command which handles the discard confirmation dialog.
            // CancelarAsync will call FecharJanela() which sets _permitirFechamento = true.
            _ = vm.CancelarCommand.ExecuteAsync(null);
        }
        else if (DataContext is ConteudoDetalheViewModel vm2 && vm2.FezAlteracoes)
        {
            // Not dirty but had prior saves: signal modified so main list refreshes.
            _permitirFechamento = true;
            WindowResult = true;
            base.OnClosing(e);
        }
        else
        {
            base.OnClosing(e);
        }
    }
}

#pragma warning restore CA2007
