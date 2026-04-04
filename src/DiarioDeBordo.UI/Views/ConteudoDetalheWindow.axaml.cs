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

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        // Set owner reference on ViewModel for Close() calls
        if (DataContext is ConteudoDetalheViewModel vm)
        {
            vm.Owner = this;
            // Trigger loading after window is shown
            vm.CarregarCommand.Execute(null);

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
                        vm.ConteudoAlvoSelecionado = vm.SugestoesConteudo
                            .FirstOrDefault(c => c.Titulo == titulo);
                };
            }

            if (this.FindControl<AutoCompleteBox>("TipoRelacaoAutoComplete") is AutoCompleteBox tipoAc)
            {
                tipoAc.AsyncPopulator = async (text, ct) =>
                {
                    await vm.PopularSugestoesTipoRelacaoAsync(text ?? string.Empty);
                    return vm.SugestoesTipoRelacao.Select(t => t.Nome).Cast<object>();
                };

                tipoAc.SelectionChanged += (_, args) =>
                {
                    if (args.AddedItems.Count > 0 && args.AddedItems[0] is string nome)
                        vm.TipoRelacaoSelecionado = vm.SugestoesTipoRelacao
                            .FirstOrDefault(t => t.Nome == nome);
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
        else
        {
            base.OnClosing(e);
        }
    }
}

#pragma warning restore CA2007
