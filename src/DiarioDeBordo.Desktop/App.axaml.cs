using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DiarioDeBordo.Core.FeatureFlags;
using DiarioDeBordo.Core.Infraestrutura;
using DiarioDeBordo.Infrastructure;
using DiarioDeBordo.Module.Acervo.Commands;
using DiarioDeBordo.UI.Services;
using DiarioDeBordo.UI.ViewModels;
using DiarioDeBordo.UI.Views;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiarioDeBordo.Desktop;

#pragma warning disable CA2007  // UI startup: no ConfigureAwait — window creation requires UI thread
#pragma warning disable CA1031  // Startup bootstrap: any exception is fatal and must be caught broadly
#pragma warning disable CA1848  // Logger perf: startup critical path, non-hot code

internal sealed partial class App : Application
{
    private IServiceProvider? _services;

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override async void OnFrameworkInitializationCompleted()
    {
        // Phase 1: minimal pre-DI container — only secure storage + PostgresBootstrap
        var preServices = new ServiceCollection();
        preServices.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Information));
        preServices.AddInfrastructureBootstrap();
        var preContainer = preServices.BuildServiceProvider();

        // Phase 2: run bootstrap to obtain the real connection string
        var bootstrap = preContainer.GetRequiredService<IPostgresBootstrap>();
        string connectionString;
        try
        {
            await bootstrap.EnsureRunningAsync(CancellationToken.None);
            connectionString = await bootstrap.BuildConnectionStringAsync(CancellationToken.None);
        }
        catch (Exception ex)
        {
            var logger = preContainer.GetRequiredService<ILogger<App>>();
            logger.LogCritical(ex, "Falha crítica ao inicializar PostgreSQL — aplicação encerrada.");
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime dl)
                dl.Shutdown(1);
            return;
        }

        // Phase 3: full DI container with the real connection string
        var services = new ServiceCollection();
        ConfigureServices(services, connectionString);
        _services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = _services.GetRequiredService<MainViewModel>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void ConfigureServices(IServiceCollection services, string connectionString)
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
#pragma warning restore CA1031
#pragma warning restore CA2007
