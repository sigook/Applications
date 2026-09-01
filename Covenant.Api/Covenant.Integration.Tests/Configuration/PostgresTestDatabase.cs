using Covenant.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Testcontainers.PostgreSql;

namespace Covenant.Integration.Tests.Configuration;

public static class PostgresTestDatabase
{
    private const string TemplateDatabase = "covenant_template";
    private const string Image = "postgres:16-alpine";

    private static readonly Lazy<string> AdminConnectionString =
        new(StartContainerAndBuildTemplate, LazyThreadSafetyMode.ExecutionAndPublication);

    private static string current;

    public static string Current => current ?? CreateDatabase();

    public static void Use(string connectionString) => current = connectionString;

    public static string CreateDatabase()
    {
        var admin = AdminConnectionString.Value;
        var database = $"test_{Guid.NewGuid():N}";
        Execute(admin, $"""CREATE DATABASE "{database}" TEMPLATE "{TemplateDatabase}";""");
        return ForDatabase(admin, database);
    }

    private static string StartContainerAndBuildTemplate()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        var container = new PostgreSqlBuilder()
            .WithImage(Image)
            .WithDatabase("postgres")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithCommand("-c", "max_connections=500", "-c", "fsync=off", "-c", "synchronous_commit=off")
            .Build();
        container.StartAsync().GetAwaiter().GetResult();

        var admin = container.GetConnectionString();
        Execute(admin, $"""CREATE DATABASE "{TemplateDatabase}";""");
        BuildSchema(ForDatabase(admin, TemplateDatabase));
        NpgsqlConnection.ClearAllPools();
        return admin;
    }

    private static void BuildSchema(string connectionString)
    {
        var options = new DbContextOptionsBuilder<CovenantContext>()
            .UseNpgsql(connectionString)
            .Options;
        using var context = new CovenantContext(options);
        context.Database.EnsureCreated();
        DatabaseScriptRunner.RunAsync(context).GetAwaiter().GetResult();
    }

    private static void Execute(string connectionString, string sql)
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    private static string ForDatabase(string connectionString, string database) =>
        new NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = database,
            IncludeErrorDetail = true,
            MaxPoolSize = 4
        }.ConnectionString;
}
