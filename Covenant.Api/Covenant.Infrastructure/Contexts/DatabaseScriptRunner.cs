using System.Data;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Contexts;

public static class DatabaseScriptRunner
{
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
            var fullDir = Path.Combine(basePath, folder);
            if (!Directory.Exists(fullDir))
            {
                continue;
            }
            foreach (var file in Directory.GetFiles(fullDir, "*.sql").OrderBy(f => f))
            {
                var sql = await File.ReadAllTextAsync(file);
                await using var command = connection.CreateCommand();
                command.CommandText = sql;
                await command.ExecuteNonQueryAsync();
            }
        }
        if (wasClosed)
        {
            await connection.CloseAsync();
        }
    }
}
