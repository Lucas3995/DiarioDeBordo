using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using DiarioDeBordo.UI.ViewModels;
using DiarioDeBordo.UI.Views;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable CA2007 // UI service: ShowDialog must stay on UI thread — no ConfigureAwait

namespace DiarioDeBordo.UI.Services;

/// <summary>
/// Implementação de IDialogService para Avalonia.
/// Inicia modais como janelas filhas da MainWindow.
/// </summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "UI dialog service — exercised via E2E tests, not unit tests.")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container.")]
internal sealed class DialogService : IDialogService
{
    private readonly IServiceProvider _serviceProvider;

    public DialogService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private static Window? GetMainWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            return desktop.MainWindow;
        return null;
    }

    public async Task<bool?> MostrarConteudoDetalheAsync(Guid conteudoId)
    {
        var mainWindow = GetMainWindow();
        if (mainWindow is null)
            return false;

        // Resolve ViewModel via factory (to get DI-injected dependencies)
        var vmFactory = _serviceProvider.GetRequiredService<Func<Guid, ConteudoDetalheViewModel>>();
        var vm = vmFactory(conteudoId);

        var window = new ConteudoDetalheWindow
        {
            DataContext = vm,
        };

        return await window.ShowDialog<bool?>(mainWindow);
    }

    public async Task<bool> MostrarConfirmacaoAsync(
        string titulo,
        string mensagem,
        string botaoPrimario,
        string botaoSecundario,
        bool isPrimarioDestructivo = false)
    {
        var mainWindow = GetMainWindow();
        if (mainWindow is null)
            return false;

        var vm = new ConfirmacaoDialogViewModel(titulo, mensagem, botaoPrimario, botaoSecundario, isPrimarioDestructivo);
        var dialog = new ConfirmacaoDialog
        {
            DataContext = vm,
        };

        return await dialog.ShowDialog<bool>(mainWindow);
    }
}

#pragma warning restore CA2007
