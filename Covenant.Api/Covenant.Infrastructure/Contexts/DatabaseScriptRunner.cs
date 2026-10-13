using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;

namespace Covenant.Infrastructure.Contexts;

public static class DatabaseScriptRunner
{
    private const string DataFolder = "Data";

    private static readonly string[] Folders =
    [
        "Schemas",
        "Tables",
        "Views",
        "Functions",
        "StoredProcedures",
        "Types",
    ];

    public static async Task RunAsync(DbContext context)
    {
        var assemblyFolder = Path.GetDirectoryName(typeof(CovenantContext).Assembly.Location)!;
        var basePath = Path.Combine(assemblyFolder, "Scripts");
        var connection = context.Database.GetDbConnection();
        var wasClosed = connection.State != ConnectionState.Open;
        if (wasClosed)
        {
            await connection.OpenAsync();
        }
        foreach (var folder in Folders)
        {
            foreach (var file in GetScripts(basePath, folder))
            {
                await ExecuteAsync(connection, await File.ReadAllTextAsync(file));
            }
        }
        await RunOnceAsync(connection, basePath);
        if (wasClosed)
        {
            await connection.CloseAsync();
        }
    }

    private static IEnumerable<string> GetScripts(string basePath, string folder)
    {
        var fullDir = Path.Combine(basePath, folder);
        return Directory.Exists(fullDir) ? Directory.GetFiles(fullDir, "*.sql").OrderBy(f => f) : [];
    }

    private static async Task RunOnceAsync(DbConnection connection, string basePath)
    {
        var scripts = GetScripts(basePath, DataFolder).ToList();
        if (scripts.Count == 0) return;

        await ExecuteAsync(connection, """
            CREATE TABLE IF NOT EXISTS "ScriptHistory" (
                "Name" text NOT NULL PRIMARY KEY,
                "AppliedAt" timestamp with time zone NOT NULL
            );
            """);

        foreach (var file in scripts)
        {
            var name = Path.GetFileName(file);
            if (await WasAppliedAsync(connection, name)) continue;

            await using var transaction = await connection.BeginTransactionAsync();
            await ExecuteAsync(connection, await File.ReadAllTextAsync(file), transaction);
            await ExecuteAsync(connection, """INSERT INTO "ScriptHistory" ("Name", "AppliedAt") VALUES (@name, now());""", transaction, name);
            await transaction.CommitAsync();
        }
    }

    private static async Task<bool> WasAppliedAsync(DbConnection connection, string name)
    {
        await using var command = CreateCommand(connection, """SELECT 1 FROM "ScriptHistory" WHERE "Name" = @name;""", null, name);
        return await command.ExecuteScalarAsync() is not null;
    }

    private static async Task ExecuteAsync(DbConnection connection, string sql, DbTransaction transaction = null, string name = null)
    {
        await using var command = CreateCommand(connection, sql, transaction, name);
        await command.ExecuteNonQueryAsync();
    }

    private static DbCommand CreateCommand(DbConnection connection, string sql, DbTransaction transaction, string name)
    {
        var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Transaction = transaction;
        if (name is null) return command;
        var parameter = command.CreateParameter();
        parameter.ParameterName = "name";
        parameter.Value = name;
        command.Parameters.Add(parameter);
        return command;
    }
}
