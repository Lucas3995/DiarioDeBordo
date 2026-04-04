using DiarioDeBordo.Core.Infraestrutura;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

using System.Diagnostics.CodeAnalysis;

namespace DiarioDeBordo.Infrastructure.Postgres;

[ExcludeFromCodeCoverage(Justification = "OS-level integration: manages pg_ctl process lifecycle — not testable without a real PostgreSQL installation. Integration tests use Testcontainers instead.")]
internal sealed partial class PostgresBootstrap : IPostgresBootstrap
{
    private readonly IArmazenamentoSeguro _secureStorage;
    private readonly ILogger<PostgresBootstrap> _logger;
    private const string PgPort = "15432";
    private const string PgDatabase = "diariodebordo";
    private const string PgUser = "postgres";
    private const string PasswordKey = "pg_password";

    public PostgresBootstrap(IArmazenamentoSeguro secureStorage, ILogger<PostgresBootstrap> logger)
    {
        _secureStorage = secureStorage;
        _logger = logger;
    }

    public async Task<bool> EnsureRunningAsync(CancellationToken ct)
    {
        try
        {
            var pgDataDir = GetPgDataDir();
            var pgBinDir = GetPgBinDir();
            LogUsingPgBinDir(_logger, pgBinDir);

            // First run: initialize cluster
            if (!await IsInitializedAsync().ConfigureAwait(false))
            {
                LogInitializingCluster(_logger, pgDataDir);
                await InitializeClusterAsync(pgDataDir, pgBinDir, ct).ConfigureAwait(false);
            }

            // Start if not running
            if (!await IsRunningAsync().ConfigureAwait(false))
            {
                LogStartingPostgres(_logger, PgPort);
                await StartAsync(pgDataDir, pgBinDir, ct).ConfigureAwait(false);

                // Wait for PostgreSQL to be ready (up to 30s)
                for (int i = 0; i < 30; i++)
                {
                    if (await IsRunningAsync().ConfigureAwait(false)) break;
                    await Task.Delay(1000, ct).ConfigureAwait(false);
                }
            }

            var running = await IsRunningAsync().ConfigureAwait(false);
            if (running)
                LogPostgresRunning(_logger, PgPort);
            else
                LogPostgresTimeout(_logger, PgPort);

            return running;
        }
#pragma warning disable CA1031 // startup failures must be caught — caller decides how to handle
        catch (Exception ex)
        {
            LogStartupFailed(_logger, ex);
            return false;
        }
#pragma warning restore CA1031
    }

    public Task<bool> IsInitializedAsync()
    {
        var pgDataDir = GetPgDataDir();
        var initialized = Directory.Exists(pgDataDir) && File.Exists(Path.Combine(pgDataDir, "PG_VERSION"));
        return Task.FromResult(initialized);
    }

    public async Task<bool> IsRunningAsync()
    {
        try
        {
            using var tcp = new System.Net.Sockets.TcpClient();
            await tcp.ConnectAsync("127.0.0.1", int.Parse(PgPort, CultureInfo.InvariantCulture)).ConfigureAwait(false);
            return true;
        }
#pragma warning disable CA1031 // TCP connect failure is expected when postgres is not running
        catch { return false; }
#pragma warning restore CA1031
    }

    public async Task<string> BuildConnectionStringAsync(CancellationToken ct)
    {
        var passwordBytes = await _secureStorage.RecuperarAsync(PasswordKey).ConfigureAwait(false);
        if (passwordBytes is null)
            throw new InvalidOperationException("PostgreSQL password not found in secure storage. Run EnsureRunningAsync first.");
        var password = Encoding.UTF8.GetString(passwordBytes);
        CryptographicOperations.ZeroMemory(passwordBytes);
        return $"Host=localhost;Port={PgPort};Database={PgDatabase};Username={PgUser};Password={password};Pooling=true;Maximum Pool Size=10";
    }

    private async Task InitializeClusterAsync(string pgDataDir, string pgBinDir, CancellationToken ct)
    {
        Directory.CreateDirectory(pgDataDir);

        // Generate and store password BEFORE initdb
        var password = GenerateStrongPassword();
        await _secureStorage.ArmazenarAsync(PasswordKey, Encoding.UTF8.GetBytes(password)).ConfigureAwait(false);

        // Write password file for initdb
        var pwFile = Path.GetTempFileName();
        try
        {
            await File.WriteAllTextAsync(pwFile, password, ct).ConfigureAwait(false);

            var initdb = Path.Combine(pgBinDir, OperatingSystem.IsWindows() ? "initdb.exe" : "initdb");
            var args = $"--pgdata=\"{pgDataDir}\" --auth=scram-sha-256 --username={PgUser} --pwfile=\"{pwFile}\" --data-checksums --encoding=UTF8 --no-instructions";

            await RunProcessAsync(initdb, args, ct).ConfigureAwait(false);
        }
        finally
        {
            if (File.Exists(pwFile))
            {
                await File.WriteAllTextAsync(pwFile, new string('0', 64), CancellationToken.None).ConfigureAwait(false); // zero before delete; CancellationToken.None: must complete even if outer ct was cancelled
                File.Delete(pwFile);
            }
            CryptographicOperations.ZeroMemory(Encoding.UTF8.GetBytes(password));
        }

        // Configure postgresql.conf
        var confPath = Path.Combine(pgDataDir, "postgresql.conf");
        var config = $"""
            port = {PgPort}
            listen_addresses = 'localhost'
            log_destination = 'stderr'
            logging_collector = off
            log_statement = 'none'
            shared_buffers = 128MB
            max_connections = 20
            """;
        await File.AppendAllTextAsync(confPath, Environment.NewLine + config, ct).ConfigureAwait(false);

        // pg_hba.conf: scram-sha-256 only
        var hbaPath = Path.Combine(pgDataDir, "pg_hba.conf");
        var hba = """
            # TYPE  DATABASE  USER  ADDRESS         METHOD
            local   all       all                   scram-sha-256
            host    all       all   127.0.0.1/32    scram-sha-256
            host    all       all   ::1/128         scram-sha-256
            """;
        await File.WriteAllTextAsync(hbaPath, hba, ct).ConfigureAwait(false);
    }

    private static async Task StartAsync(string pgDataDir, string pgBinDir, CancellationToken ct)
    {
        var pgCtl = Path.Combine(pgBinDir, OperatingSystem.IsWindows() ? "pg_ctl.exe" : "pg_ctl");
        var logFile = Path.Combine(pgDataDir, "postgresql.log");
        var args = $"start --pgdata=\"{pgDataDir}\" --log=\"{logFile}\" --wait";
        await RunProcessAsync(pgCtl, args, ct).ConfigureAwait(false);
    }

    private static async Task RunProcessAsync(string executable, string args, CancellationToken ct)
    {
        using var proc = new Process();
        proc.StartInfo = new ProcessStartInfo(executable, args)
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
        proc.Start();
        await proc.WaitForExitAsync(ct).ConfigureAwait(false);
        if (proc.ExitCode != 0)
        {
            var stderr = await proc.StandardError.ReadToEndAsync(ct).ConfigureAwait(false);
            throw new InvalidOperationException($"Process '{executable}' failed (exit {proc.ExitCode}): {stderr}");
        }
    }

    private static string GenerateStrongPassword()
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";
        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return new string(bytes.Select(b => chars[b % chars.Length]).ToArray());
    }

    private static string GetPgBinDir()
    {
#if DEBUG
        var systemBin = FindSystemPgBinDir();
        if (systemBin is not null)
            return systemBin;
#endif
        return Path.Combine(AppContext.BaseDirectory, "installer", "postgres", "bin");
    }

#if DEBUG
    /// <summary>
    /// Localiza o diretório bin do PostgreSQL instalado no sistema.
    /// Usado apenas em Debug para facilitar o desenvolvimento local sem o bundled Postgres.
    /// </summary>
    private static string? FindSystemPgBinDir()
    {
        if (OperatingSystem.IsLinux())
        {
            const string pgBase = "/usr/lib/postgresql";
            if (!Directory.Exists(pgBase)) return null;

            return Directory.GetDirectories(pgBase)
                .Select(d => (path: Path.Combine(d, "bin"), ver: int.TryParse(Path.GetFileName(d), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : 0))
                .Where(x => Directory.Exists(x.path) && File.Exists(Path.Combine(x.path, "pg_ctl")))
                .OrderByDescending(x => x.ver)
                .Select(x => x.path)
                .FirstOrDefault();
        }

        if (OperatingSystem.IsWindows())
        {
            const string pgBase = @"C:\Program Files\PostgreSQL";
            if (!Directory.Exists(pgBase)) return null;

            return Directory.GetDirectories(pgBase)
                .Select(d => (path: Path.Combine(d, "bin"), ver: int.TryParse(Path.GetFileName(d), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : 0))
                .Where(x => Directory.Exists(x.path) && File.Exists(Path.Combine(x.path, "pg_ctl.exe")))
                .OrderByDescending(x => x.ver)
                .Select(x => x.path)
                .FirstOrDefault();
        }

        return null;
    }
#endif

    private static string GetPgDataDir()
    {
        var dataHome = OperatingSystem.IsWindows()
            ? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            : Environment.GetEnvironmentVariable("XDG_DATA_HOME")
              ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share");
        var dir = Path.Combine(dataHome, "DiarioDeBordo", "pgdata");
        Directory.CreateDirectory(dir);
        return dir;
    }

    // LoggerMessage delegates — avoids CA1848/CA1873 boxing allocation warnings
    [LoggerMessage(Level = LogLevel.Information, Message = "Using PostgreSQL bin directory: {PgBinDir}")]
    private static partial void LogUsingPgBinDir(ILogger logger, string pgBinDir);

    [LoggerMessage(Level = LogLevel.Information, Message = "Initializing PostgreSQL cluster at {DataDir}")]
    private static partial void LogInitializingCluster(ILogger logger, string dataDir);

    [LoggerMessage(Level = LogLevel.Information, Message = "Starting PostgreSQL on port {Port}")]
    private static partial void LogStartingPostgres(ILogger logger, string port);

    [LoggerMessage(Level = LogLevel.Information, Message = "PostgreSQL is running on port {Port}")]
    private static partial void LogPostgresRunning(ILogger logger, string port);

    [LoggerMessage(Level = LogLevel.Warning, Message = "PostgreSQL did not start within timeout on port {Port}")]
    private static partial void LogPostgresTimeout(ILogger logger, string port);

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to ensure PostgreSQL is running")]
    private static partial void LogStartupFailed(ILogger logger, Exception exception);
}
