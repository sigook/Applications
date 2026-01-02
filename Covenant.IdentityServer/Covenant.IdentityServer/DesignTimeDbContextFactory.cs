using Covenant.IdentityServer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Covenant.IdentityServer
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MyKeysContext>
    {
        public MyKeysContext CreateDbContext(string[] args)
        {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", true, true)
                .AddEnvironmentVariables()
                .Build();
            var builder = new DbContextOptionsBuilder<MyKeysContext>();
            string connectionString = config.GetConnectionString("DefaultConnection");
            builder.UseNpgsql(connectionString);
            return new MyKeysContext(builder.Options);
        }
    }
}