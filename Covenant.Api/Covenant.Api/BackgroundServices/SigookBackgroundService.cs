using Azure.Messaging.ServiceBus.Administration;
using Covenant.Common.Configuration;
using Covenant.Common.Constants;
using Covenant.Common.Interfaces;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Covenant.Api.BackgroundServices;

public class SigookBackgroundService : BackgroundService
{
    private readonly IServiceProvider serviceProvider;

    public SigookBackgroundService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ConfigureMigrations();
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

    private async Task ConfigureServiceBus()
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var client = scope.ServiceProvider.GetRequiredService<SigookBusAdministrationClient>();
        var configuration = scope.ServiceProvider.GetRequiredService<IOptions<ServiceBusConfiguration>>().Value;
        var consumers = scope.ServiceProvider.GetServices<IAzureServiceBusConsumer>();
        await client.CreateQueueIfNotExistsAsync(configuration.ValidateCandidateQueue);
        await client.CreateQueueIfNotExistsAsync(configuration.BulkPayStubEmailQueue);
        await client.CreateQueueIfNotExistsAsync(configuration.InvitationQueue);
        await client.CreateTopicIfNotExistsAsync(configuration.CreateApplicantTopic);
        await client.CreateSubscriptionIfNotExistsAsync(configuration.CreateApplicantTopic, TopicSubscription.TeamsNotification);
        await client.CreateSubscriptionIfNotExistsAsync(configuration.CreateApplicantTopic, TopicSubscription.RequestApplicantNotification);
        await client.CreateRuleIfNotExistsAsync(configuration.CreateApplicantTopic, TopicSubscription.RequestApplicantNotification,
            "OnCandidateCreated",
            new SqlRuleFilter($"{ServiceBusSqlConstants.RequestApplication} IS NOT NULL")
        );
        foreach (var consumer in consumers)
        {
            await consumer.OnInit();
        }
    }
}
