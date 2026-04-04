using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DiarioDeBordo.Core.FeatureFlags;
using DiarioDeBordo.Infrastructure;
using DiarioDeBordo.Module.Acervo.Commands;
using DiarioDeBordo.UI.Services;
using DiarioDeBordo.UI.ViewModels;
using DiarioDeBordo.UI.Views;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiarioDeBordo.Desktop;

#pragma warning disable CA1848  // Logger perf: startup critical path, non-hot code

internal sealed partial class App : Application
{
    private readonly IServiceProvider _services;

    public App(IServiceProvider services)
    {
        _services = services;
    }

    // Parameterless constructor required by Avalonia's XAML loader
    public App() => throw new InvalidOperationException(
        "App must be constructed via BuildAvaloniaApp(IServiceProvider).");

    public static AppBuilder BuildAvaloniaApp(IServiceProvider services)
        => AppBuilder.Configure(() => new App(services))
            .UsePlatformDetect()
            .LogToTrace();

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = _services.GetRequiredService<MainViewModel>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static void ConfigureServices(IServiceCollection services, string connectionString)
    {
        services.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Information));

        // Infrastructure: DbContext, repositories, query services
        services.AddInfrastructure(connectionString);

        // MediatR: scan Module.Acervo for all handlers
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(typeof(CriarConteudoCommand).Assembly));

        // Feature flags placeholder (Phase 2 — all flags off until Phase 3)
        services.AddSingleton<IFeatureFlags, FeatureFlagsPlaceholder>();

        // ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<AcervoViewModel>();
        services.AddTransient<CriarConteudoViewModel>();
        services.AddTransient<ConteudoDetalheViewModel>();

        // Dialog service (singleton — manages reference to main window)
        services.AddSingleton<IDialogService, DiarioDeBordo.UI.Services.DialogService>();

        // ViewModel factories — allow DI-resolved instances on demand
        services.AddTransient<Func<AcervoViewModel>>(
            sp => () => sp.GetRequiredService<AcervoViewModel>());
        services.AddTransient<Func<CriarConteudoViewModel>>(
            sp => () => sp.GetRequiredService<CriarConteudoViewModel>());

        // ConteudoDetalheViewModel factory (keyed by conteudoId)
        services.AddTransient<Func<Guid, ConteudoDetalheViewModel>>(sp =>
            conteudoId => new ConteudoDetalheViewModel(
                sp.GetRequiredService<IMediator>(),
                conteudoId,
                sp.GetRequiredService<IDialogService>()));
    }
}

#pragma warning restore CA1848
