using Azure.Messaging.ServiceBus.Administration;
using Covenant.Common.Configuration;
using Covenant.Common.Constants;
using Covenant.Common.Interfaces;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;

namespace Covenant.Api.BackgroundServices
{
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
            await RunCustomScriptsAsync(covenantContext);
        }

        private async Task ConfigureServiceBus()
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            var client = scope.ServiceProvider.GetRequiredService<SigookBusAdministrationClient>();
            var configuration = scope.ServiceProvider.GetRequiredService<IOptions<ServiceBusConfiguration>>().Value;
            var consumers = scope.ServiceProvider.GetServices<IAzureServiceBusConsumer>();
            await client.CreateQueueIfNotExistsAsync(configuration.ValidateCandidateQueue);
            await client.CreateTopicIfNotExistsAsync(configuration.CreateApplicantTopic);
            await client.CreateSubscriptionIfNotExistsAsync(configuration.CreateApplicantTopic, TopicSubscription.TeamsNotification);
            await client.CreateSubscriptionIfNotExistsAsync(configuration.CreateApplicantTopic, TopicSubscription.EmailNotification);
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

        private async Task RunCustomScriptsAsync(DbContext context)
        {
            var infraAssembly = typeof(CovenantContext).Assembly;
            var assemblyFolder = Path.GetDirectoryName(infraAssembly.Location)!;
            var basePath = Path.Combine(assemblyFolder, "Scripts");
            var connection = context.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync();
            }
            var folders = new[]
            {
                "Schemas",
                "Tables",
                "Views",
                "Functions",
                "StoredProcedures",
                "Types",
            };
            foreach (var folder in folders)
            {
                var fullDir = Path.Combine(basePath, folder);
                if (!Directory.Exists(fullDir))
                {
                    continue;
                }
                var files = Directory.GetFiles(fullDir, "*.sql").OrderBy(f => f);
                foreach (var file in files)
                {
                    var sql = await File.ReadAllTextAsync(file);
                    using var cmd = connection.CreateCommand();
                    cmd.CommandText = sql;
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            await connection.CloseAsync();
        }
    }
}
