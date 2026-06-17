using Azure.Messaging.ServiceBus;
using Covenant.Common.Configuration;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Models.Notification;
using Covenant.Core.BL.Interfaces;
using Covenant.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Covenant.Core.BL.Consumers;

public class BulkPayStubEmailConsumer : IAzureServiceBusConsumer
{
    private const int MaxConcurrency = 5;
    private readonly ISigookBusClient client;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly ServiceBusConfiguration serviceBusConfiguration;
    private readonly ILogger<BulkPayStubEmailConsumer> logger;

    public BulkPayStubEmailConsumer(
        ISigookBusClient client,
        IServiceScopeFactory serviceScopeFactory,
        IOptions<ServiceBusConfiguration> options,
        ILogger<BulkPayStubEmailConsumer> logger)
    {
        this.client = client;
        this.serviceScopeFactory = serviceScopeFactory;
        serviceBusConfiguration = options.Value;
        this.logger = logger;
    }

    public async Task OnInit()
    {
        await client.CreateProcessorAsync(serviceBusConfiguration.BulkPayStubEmailQueue, ProcessAsync);
    }

    private async Task ProcessAsync(ProcessMessageEventArgs args)
    {
        var job = args.Message.Body.ToObjectFromJson<BulkPayStubEmailJob>();
        using var throttler = new SemaphoreSlim(MaxConcurrency);
        var tasks = job.PayStubIds.Select(payStubId => SendPayStubEmail(payStubId, throttler, args.CancellationToken));
        var results = await Task.WhenAll(tasks);
        await NotifyTeams(job, results);
        await args.CompleteMessageAsync(args.Message);
    }

    private async Task<PayStubEmailResult> SendPayStubEmail(Guid payStubId, SemaphoreSlim throttler, CancellationToken cancellationToken)
    {
        await throttler.WaitAsync(cancellationToken);
        try
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var payStubService = scope.ServiceProvider.GetRequiredService<IPayStubService>();
            return await payStubService.SendPayStubEmail(payStubId);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to send pay stub {PayStubId}", payStubId);
            return PayStubEmailResult.Failed(payStubId);
        }
        finally
        {
            throttler.Release();
        }
    }

    private async Task NotifyTeams(BulkPayStubEmailJob job, IReadOnlyCollection<PayStubEmailResult> results)
    {
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var teamsService = scope.ServiceProvider.GetRequiredService<ITeamsService>();
        var webhook = scope.ServiceProvider.GetRequiredService<IOptions<TeamsWebhookConfiguration>>().Value.Accounting;

        var failed = results.Where(r => !r.Success).ToList();
        int sentCount = results.Count - failed.Count;
        string text = $"Sent {sentCount} of {results.Count}";
        if (failed.Count > 0)
        {
            string failedNames = string.Join(" - ", failed.Select(f =>
                string.IsNullOrEmpty(f.WorkerFullName) ? f.PayStubId.ToString() : f.WorkerFullName));
            text = $"{text} - Failed: {failedNames}";
        }

        var notification = failed.Count > 0
            ? TeamsNotificationModel.CreateWarning($"PayStubs sent by {job.Nickname}", text)
            : TeamsNotificationModel.CreateSuccess($"PayStubs sent by {job.Nickname}", text);
        await teamsService.SendNotification(webhook, notification);
    }
}
