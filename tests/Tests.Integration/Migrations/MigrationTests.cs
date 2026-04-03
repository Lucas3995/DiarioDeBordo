using DiarioDeBordo.Tests.Integration.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DiarioDeBordo.Tests.Integration.Migrations;

/// <summary>
/// Tests that EF Core migrations apply correctly to a real PostgreSQL instance.
/// Meurice et al. (2015, MSR) — schema-code divergence is a leading cause of production failures.
/// </summary>
public class MigrationTests : PostgresTestBase
{
    [Fact]
    public async Task Migration_Up_DatabaseAceitaConexao()
    {
        Assert.True(await Context.Database.CanConnectAsync());
    }

    [Fact]
    public async Task Migration_Up_CriaTabela_Conteudos()
    {
        var tables = await ObterTabelasAsync();
        Assert.Contains("conteudos", tables);
    }

    [Fact]
    public async Task Migration_Up_CriaTabela_Fontes()
    {
        var tables = await ObterTabelasAsync();
        Assert.Contains("fontes", tables);
    }

    [Fact]
    public async Task Migration_Up_CriaTabela_ImagensConteudo()
    {
        var tables = await ObterTabelasAsync();
        Assert.Contains("imagens_conteudo", tables);
    }

    [Fact]
    public async Task Migration_Up_CriaTabela_Categorias()
    {
        var tables = await ObterTabelasAsync();
        Assert.Contains("categorias", tables);
    }

    [Fact]
    public async Task Migration_Up_Conteudos_TemColuna_UsuarioId()
    {
        var columns = await ObterColunasAsync("conteudos");
        Assert.Contains("usuario_id", columns);
    }

    [Fact]
    public async Task Migration_Up_Conteudos_TemColuna_Titulo()
    {
        var columns = await ObterColunasAsync("conteudos");
        Assert.Contains("titulo", columns);
    }

    [Fact]
    public async Task Migration_Up_IndiceUnicoFontes_Prioridade()
    {
        var indexes = await ObterIndicesAsync("fontes");
        Assert.Contains("idx_fontes_conteudo_prioridade_unique", indexes);
    }

    [Fact]
    public async Task Migration_ReAplicada_NaoFalha()
    {
        // Idempotency check — applying migrations twice should not fail
        await Context.Database.MigrateAsync();
        Assert.True(await Context.Database.CanConnectAsync());
    }

    private async Task<IReadOnlyList<string>> ObterTabelasAsync()
    {
        var conn = Context.Database.GetDbConnection();
        await conn.OpenAsync();
        try
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                "SELECT tablename FROM pg_tables WHERE schemaname = 'public' ORDER BY tablename";
            using var reader = await cmd.ExecuteReaderAsync();
            var tables = new List<string>();
            while (await reader.ReadAsync())
                tables.Add(reader.GetString(0));
            return tables;
        }
        finally
        {
            await conn.CloseAsync();
        }
    }

    private async Task<IReadOnlyList<string>> ObterColunasAsync(string tableName)
    {
        var conn = Context.Database.GetDbConnection();
        await conn.OpenAsync();
        try
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                "SELECT column_name FROM information_schema.columns " +
                "WHERE table_schema = 'public' AND table_name = @t";
            var param = cmd.CreateParameter();
            param.ParameterName = "@t";
            param.Value = tableName;
            cmd.Parameters.Add(param);
            using var reader = await cmd.ExecuteReaderAsync();
            var columns = new List<string>();
            while (await reader.ReadAsync())
                columns.Add(reader.GetString(0));
            return columns;
        }
        finally
        {
            await conn.CloseAsync();
        }
    }

    private async Task<IReadOnlyList<string>> ObterIndicesAsync(string tableName)
    {
        var conn = Context.Database.GetDbConnection();
        await conn.OpenAsync();
        try
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                "SELECT indexname FROM pg_indexes " +
                "WHERE schemaname = 'public' AND tablename = @t";
            var param = cmd.CreateParameter();
            param.ParameterName = "@t";
            param.Value = tableName;
            cmd.Parameters.Add(param);
            using var reader = await cmd.ExecuteReaderAsync();
            var indexes = new List<string>();
            while (await reader.ReadAsync())
                indexes.Add(reader.GetString(0));
            return indexes;
        }
        finally
        {
            await conn.CloseAsync();
        }
    }
}
