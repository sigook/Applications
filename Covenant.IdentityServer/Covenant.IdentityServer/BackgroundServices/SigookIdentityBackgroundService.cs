using Covenant.IdentityServer.Data;
using Covenant.IdentityServer.Entities;
using Covenant.IdentityServer.Security;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Covenant.IdentityServer.BackgroundServices;

public class SigookIdentityBackgroundService : BackgroundService
{
    private static readonly string[] Roles =
    [
        RoleConstants.SuperAdmin,
        RoleConstants.Admin,
        RoleConstants.Recruiting,
        RoleConstants.Sales,
        RoleConstants.Company,
        RoleConstants.CompanyUser,
        RoleConstants.Worker
    ];

    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<SigookIdentityBackgroundService> logger;

    public SigookIdentityBackgroundService(IServiceProvider serviceProvider, ILogger<SigookIdentityBackgroundService> logger)
    {
        this.serviceProvider = serviceProvider;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ConfigureMigrations();
        await SeedRoles();
    }

    private async Task ConfigureMigrations()
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var myKeysContext = scope.ServiceProvider.GetRequiredService<MyKeysContext>();
        if ((await myKeysContext.Database.GetPendingMigrationsAsync()).Any())
        {
            await myKeysContext.Database.MigrateAsync();
        }

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

    private async Task SeedRoles()
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<CovenantRole>>();

        foreach (string role in Roles)
        {
            CovenantRole existing = await roleManager.Roles.FirstOrDefaultAsync(r => r.Name == role);
            if (existing is null)
            {
                IdentityResult created = await roleManager.CreateAsync(new CovenantRole { Name = role });
                if (created.Succeeded)
                    logger.LogInformation("Role created: {Role}", role);
                else
                    logger.LogError("Error creating role {Role}: {Errors}", role, string.Join(", ", created.Errors.Select(e => e.Description)));
                continue;
            }

            string normalizedName = roleManager.NormalizeKey(role);
            if (existing.NormalizedName == normalizedName) continue;

            existing.NormalizedName = normalizedName;
            IdentityResult updated = await roleManager.UpdateAsync(existing);
            if (updated.Succeeded)
                logger.LogInformation("Role normalized name fixed: {Role}", role);
            else
                logger.LogError("Error normalizing role {Role}: {Errors}", role, string.Join(", ", updated.Errors.Select(e => e.Description)));
        }
    }
}
