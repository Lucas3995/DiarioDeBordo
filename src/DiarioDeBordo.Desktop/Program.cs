using Avalonia;
using Avalonia.Controls;
using DiarioDeBordo.Core.Infraestrutura;
using DiarioDeBordo.Infrastructure;
using DiarioDeBordo.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiarioDeBordo.Desktop;

internal static class Program
{
    [System.STAThread]
    public static int Main(string[] args)
    {
        // On Linux, IBus (XMODIFIERS=@im=ibus) intercepts dead keys at the X11 level
        // but Avalonia 11's XIM integration doesn't properly receive the composed
        // characters back (e.g., ´ + a → á). Bypassing IBus lets XKB handle dead key
        // composition directly via XLookupString, which works correctly.
        if (OperatingSystem.IsLinux())
            Environment.SetEnvironmentVariable("XMODIFIERS", "@im=none");

        // Bootstrap PostgreSQL and apply migrations BEFORE Avalonia starts.
        // This avoids threading issues — Avalonia requires all UI operations on its
        // UI thread, which hasn't been set up yet at this point.
        IServiceProvider? services = null;
        try
        {
            services = BootstrapAsync().GetAwaiter().GetResult();
        }
#pragma warning disable CA1031 // bootstrap failures are fatal — catch broadly
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Falha crítica na inicialização: {ex.Message}");
            return 1;
        }
#pragma warning restore CA1031

        return App.BuildAvaloniaApp(services)
            .StartWithClassicDesktopLifetime(args);
    }

    private static async Task<IServiceProvider> BootstrapAsync()
    {
        // Phase 1: minimal container for secure storage + Postgres bootstrap
        var preServices = new ServiceCollection();
        preServices.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Information));
        preServices.AddInfrastructureBootstrap();
        var preContainer = preServices.BuildServiceProvider();

        // Phase 2: start PostgreSQL and get connection string
        var bootstrap = preContainer.GetRequiredService<IPostgresBootstrap>();
        await bootstrap.EnsureRunningAsync(CancellationToken.None).ConfigureAwait(false);
        var connectionString = await bootstrap.BuildConnectionStringAsync(CancellationToken.None).ConfigureAwait(false);

        // Phase 3: full service container
        var services = new ServiceCollection();
        App.ConfigureServices(services, connectionString);
        var serviceProvider = services.BuildServiceProvider();

        // Phase 4: apply pending EF Core migrations (creates DB + schema if needed)
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DiarioDeBordoDbContext>();
        await db.Database.MigrateAsync().ConfigureAwait(false);

        return serviceProvider;
    }
}

