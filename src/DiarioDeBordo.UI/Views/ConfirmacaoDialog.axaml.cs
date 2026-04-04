using Avalonia.Controls;
using Avalonia.Interactivity;
using DiarioDeBordo.UI.ViewModels;

namespace DiarioDeBordo.UI.Views;

/// <summary>Diálogo de confirmação genérico para ações destrutivas (D-02, D-03).</summary>
public partial class ConfirmacaoDialog : Window
{
    public ConfirmacaoDialog()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        // Wire button click handlers after AXAML is loaded
        if (this.FindControl<Button>("BotaoPrimarioBtn") is Button primario)
            primario.Click += (_, _) => Close(true);

        if (this.FindControl<Button>("BotaoSecundarioBtn") is Button secundario)
            secundario.Click += (_, _) => Close(false);
    }
}
