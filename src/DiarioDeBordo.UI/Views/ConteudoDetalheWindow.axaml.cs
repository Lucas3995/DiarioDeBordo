using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DiarioDeBordo.UI.ViewModels;

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
