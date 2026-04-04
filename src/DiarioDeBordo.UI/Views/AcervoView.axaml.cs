using Avalonia.Controls;
using Avalonia.Interactivity;
using DiarioDeBordo.UI.ViewModels;

namespace DiarioDeBordo.UI.Views;

public partial class AcervoView : UserControl
{
    public AcervoView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is AcervoViewModel vm)
            vm.CarregarCommand.Execute(null);
    }
}
