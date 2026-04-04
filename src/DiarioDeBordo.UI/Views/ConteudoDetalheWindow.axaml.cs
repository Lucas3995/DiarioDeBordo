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
    public ConteudoDetalheWindow()
    {
        InitializeComponent();
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

            // Wire AutoCompleteBox Populating events for async suggestions
            if (this.FindControl<AutoCompleteBox>("CategoriaAutoComplete") is AutoCompleteBox categoriaAc)
            {
                categoriaAc.Populating += async (_, args) =>
                    await vm.PopularSugestoesCategoriasAsync(args.Parameter ?? string.Empty);

                // Handle selection completed
                categoriaAc.SelectionChanged += async (_, args) =>
                {
                    if (args.AddedItems.Count > 0 && args.AddedItems[0] is DiarioDeBordo.Module.Acervo.DTOs.CategoriaDto dto)
                    {
                        await vm.SelecionarCategoriaAsync(dto.Nome);
                        categoriaAc.Text = string.Empty;
                    }
                };
            }

            if (this.FindControl<AutoCompleteBox>("ConteudoAlvoAutoComplete") is AutoCompleteBox conteudoAc)
            {
                conteudoAc.Populating += async (_, args) =>
                    await vm.PopularSugestoesConteudoAsync(args.Parameter ?? string.Empty);

                conteudoAc.SelectionChanged += (_, args) =>
                {
                    if (args.AddedItems.Count > 0 && args.AddedItems[0] is DiarioDeBordo.Module.Acervo.DTOs.ConteudoResumoDto dto)
                        vm.ConteudoAlvoSelecionado = dto;
                };
            }

            if (this.FindControl<AutoCompleteBox>("TipoRelacaoAutoComplete") is AutoCompleteBox tipoAc)
            {
                tipoAc.Populating += async (_, args) =>
                    await vm.PopularSugestoesTipoRelacaoAsync(args.Parameter ?? string.Empty);

                tipoAc.SelectionChanged += (_, args) =>
                {
                    if (args.AddedItems.Count > 0 && args.AddedItems[0] is DiarioDeBordo.Module.Acervo.DTOs.TipoRelacaoDto dto)
                        vm.TipoRelacaoSelecionado = dto;
                };
            }
        }
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        // Intercept close (X button) to show discard confirmation if dirty
        if (DataContext is ConteudoDetalheViewModel vm && vm.IsDirty)
        {
            e.Cancel = true;
            // Trigger cancel command which handles the discard confirmation dialog
            _ = vm.CancelarCommand.ExecuteAsync(null);
        }
        else
        {
            base.OnClosing(e);
        }
    }
}

#pragma warning restore CA2007
