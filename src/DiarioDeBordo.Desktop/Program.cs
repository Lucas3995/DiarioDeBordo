using Avalonia;
using Avalonia.Controls;

namespace DiarioDeBordo.Desktop;

internal static class Program
{
    [System.STAThread]
    public static int Main(string[] args) =>
        App.BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
}

