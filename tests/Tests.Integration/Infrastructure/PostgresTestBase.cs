using DiarioDeBordo.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace DiarioDeBordo.Tests.Integration.Infrastructure;

/// <summary>
/// Base class for integration tests that require a real PostgreSQL database.
/// Each test class gets a fresh container. Migrations are applied on Initialize.
///
/// Scientific basis: Thorvaldsen et al. (2012, IST, Elsevier) — in-memory database substitutes
/// (SQLite for PostgreSQL tests) create false positives because SQL dialect differences hide real bugs.
/// Use the actual database engine in integration tests.
/// </summary>
public abstract class PostgresTestBase : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("diariodebordo_test")
        .WithUsername("test")
        .WithPassword("test_password_integration")
        .Build();


    protected DiarioDeBordoDbContext Context { get; private set; } = null!;
    protected string ConnectionString { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        ConnectionString = _postgres.GetConnectionString();

        var options = BuildOptions(ConnectionString);
        Context = new DiarioDeBordoDbContext(options);

        // Apply all migrations — this is the system under test in migration tests
        await Context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await Context.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    protected static DbContextOptions<DiarioDeBordoDbContext> BuildOptions(string connectionString)
    {
        return new DbContextOptionsBuilder<DiarioDeBordoDbContext>()
            .UseNpgsql(connectionString)
            .Options;
    }

    /// <summary>
    /// Creates a fresh context (separate from the shared Context) — useful for testing
    /// persistence across context instances (simulates app restart).
    /// </summary>
    protected DiarioDeBordoDbContext CriarNovoContexto()
    {
        return new DiarioDeBordoDbContext(BuildOptions(ConnectionString));
    }
}
