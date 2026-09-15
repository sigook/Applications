using Azure.Messaging.ServiceBus.Administration;
using Covenant.Api.Configuration;
using Covenant.Common.Configuration;
using Covenant.Common.Constants;
using Covenant.Common.Entities.Identity;
using Covenant.Common.Interfaces;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Covenant.Api.BackgroundServices;

public class SigookBackgroundService(
    IServiceProvider serviceProvider,
    IConfiguration configuration,
    ILogger<SigookBackgroundService> logger) : BackgroundService
{
    private static readonly string[] Roles =
    [
        CovenantConstants.Role.SuperAdmin,
        CovenantConstants.Role.Admin,
        CovenantConstants.Role.Recruiting,
        CovenantConstants.Role.Sales,
        CovenantConstants.Role.Company,
        CovenantConstants.Role.CompanyUser,
        CovenantConstants.Role.Worker
    ];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ConfigureMigrations();
        await ConfigureIdentity();
        await ConfigureServiceBus();
    }

    private async Task ConfigureMigrations()
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var myKeysContext = scope.ServiceProvider.GetRequiredService<MyKeysContext>();
        var covenantContext = scope.ServiceProvider.GetRequiredService<CovenantContext>();
        if ((await myKeysContext.Database.GetPendingMigrationsAsync()).Any())
        {
            await myKeysContext.Database.MigrateAsync();
        }
        if ((await covenantContext.Database.GetPendingMigrationsAsync()).Any())
        {
            await covenantContext.Database.MigrateAsync();
        }
        await DatabaseScriptRunner.RunAsync(covenantContext);
    }

    private async Task ConfigureIdentity()
    {
        if (string.IsNullOrEmpty(configuration.GetConnectionString(ApiServicesConfiguration.IdentityConnection)))
        {
            logger.LogWarning("Identity connection string is missing. Identity migrations and role seeding were skipped.");
            return;
        }

        await using var scope = serviceProvider.CreateAsyncScope();
        var identityContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();
        if ((await identityContext.Database.GetPendingMigrationsAsync()).Any())
        {
            await identityContext.Database.MigrateAsync();
        }
        await SeedRoles(scope.ServiceProvider.GetRequiredService<RoleManager<CovenantRole>>());
        await scope.ServiceProvider.GetRequiredService<OpenIddictSeeder>().Seed();
    }

    private async Task SeedRoles(RoleManager<CovenantRole> roleManager)
    {
        foreach (var role in Roles)
        {
            var existing = await roleManager.Roles.FirstOrDefaultAsync(r => r.Name == role);
            if (existing is null)
            {
                var created = await roleManager.CreateAsync(new CovenantRole { Name = role });
                if (created.Succeeded)
                    logger.LogInformation("Role created: {Role}", role);
                else
                    logger.LogError("Error creating role {Role}: {Errors}", role, string.Join(", ", created.Errors.Select(e => e.Description)));
                continue;
            }

            var normalizedName = roleManager.NormalizeKey(role);
            if (existing.NormalizedName == normalizedName) continue;

            existing.NormalizedName = normalizedName;
            var updated = await roleManager.UpdateAsync(existing);
            if (updated.Succeeded)
                logger.LogInformation("Role normalized name fixed: {Role}", role);
            else
                logger.LogError("Error normalizing role {Role}: {Errors}", role, string.Join(", ", updated.Errors.Select(e => e.Description)));
        }
    }

    private async Task ConfigureServiceBus()
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var client = scope.ServiceProvider.GetRequiredService<SigookBusAdministrationClient>();
        var busConfiguration = scope.ServiceProvider.GetRequiredService<IOptions<ServiceBusConfiguration>>().Value;
        var consumers = scope.ServiceProvider.GetServices<IAzureServiceBusConsumer>();
        await client.CreateQueueIfNotExistsAsync(busConfiguration.ValidateCandidateQueue);
        await client.CreateQueueIfNotExistsAsync(busConfiguration.BulkPayStubEmailQueue);
        await client.CreateQueueIfNotExistsAsync(busConfiguration.InvitationQueue);
        await client.CreateTopicIfNotExistsAsync(busConfiguration.CreateApplicantTopic);
        await client.CreateSubscriptionIfNotExistsAsync(busConfiguration.CreateApplicantTopic, TopicSubscription.TeamsNotification);
        await client.CreateSubscriptionIfNotExistsAsync(busConfiguration.CreateApplicantTopic, TopicSubscription.RequestApplicantNotification);
        await client.CreateRuleIfNotExistsAsync(busConfiguration.CreateApplicantTopic, TopicSubscription.RequestApplicantNotification,
            "OnCandidateCreated",
            new SqlRuleFilter($"{ServiceBusSqlConstants.RequestApplication} IS NOT NULL")
        );
        foreach (var consumer in consumers)
        {
            await consumer.OnInit();
        }
    }
}
