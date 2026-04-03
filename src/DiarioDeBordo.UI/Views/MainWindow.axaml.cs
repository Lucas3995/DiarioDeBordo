using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DiarioDeBordo.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
