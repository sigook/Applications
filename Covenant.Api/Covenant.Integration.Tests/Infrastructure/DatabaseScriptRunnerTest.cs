using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Integration.Tests.Infrastructure;

public class DatabaseScriptRunnerTest
{
    private static CovenantContext CreateContext(string connectionString) =>
        new(new DbContextOptionsBuilder<CovenantContext>().UseNpgsql(connectionString).Options);

    [Fact]
    public async Task DataScripts_RunOnce_AndAreRecordedInScriptHistory()
    {
        await using var context = CreateContext(PostgresTestDatabase.CreateDatabase());
        var table = $"probe_{Guid.NewGuid():N}";
        var script = $"{table}.sql";
        Directory.CreateDirectory(DatabaseScriptRunner.DataPath);
        var path = Path.Combine(DatabaseScriptRunner.DataPath, script);
        await File.WriteAllTextAsync(path, $"""CREATE TABLE "{table}" ("Id" integer NOT NULL PRIMARY KEY);""");

        try
        {
            await DatabaseScriptRunner.RunAsync(context);
            await DatabaseScriptRunner.RunAsync(context);
        }
        finally
        {
            File.Delete(path);
        }

        var applied = await context.Database
            .SqlQueryRaw<string>("""SELECT "Name" AS "Value" FROM "ScriptHistory" WHERE "Name" = {0};""", script)
            .ToListAsync();
        Assert.Single(applied);

        var tables = await context.Database
            .SqlQueryRaw<string>("""SELECT table_name AS "Value" FROM information_schema.tables WHERE table_name = {0};""", table)
            .ToListAsync();
        Assert.Single(tables);
    }
}
