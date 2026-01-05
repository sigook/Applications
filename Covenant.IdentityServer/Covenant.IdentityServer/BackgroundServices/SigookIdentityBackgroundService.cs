

using Covenant.IdentityServer.Data;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Covenant.IdentityServer.BackgroundServices;

public class SigookIdentityBackgroundService : BackgroundService
{
    private readonly IServiceProvider serviceProvider;

    public SigookIdentityBackgroundService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ConfigureMigrations();
    }

    private async Task ConfigureMigrations()
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var covenantContext = scope.ServiceProvider.GetRequiredService<CovenantContext>();
        if ((await covenantContext.Database.GetPendingMigrationsAsync()).Any())
        {
            await covenantContext.Database.MigrateAsync();
        }

        var persistedGrantDb = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
        if ((await persistedGrantDb.Database.GetPendingMigrationsAsync()).Any())
        {
            await persistedGrantDb.Database.MigrateAsync();
        }

        var configurationDb = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
        if ((await configurationDb.Database.GetPendingMigrationsAsync()).Any())
        {
            await configurationDb.Database.MigrateAsync();
        }
    }
}
