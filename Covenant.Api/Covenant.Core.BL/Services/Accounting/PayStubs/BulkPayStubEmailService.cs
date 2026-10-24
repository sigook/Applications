using Covenant.Common.Configuration;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Accounting.PayStubs;
using Covenant.Common.Models.Notifications;
using Covenant.Core.BL.Interfaces.Accounting.PayStubs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Covenant.Core.BL.Services.Accounting.PayStubs;

public class BulkPayStubEmailService(
    IServiceScopeFactory serviceScopeFactory,
    ITeamsService teamsService,
    IOptions<TeamsWebhookConfiguration> teamsWebhookOptions,
    ILogger<BulkPayStubEmailService> logger) : IBulkPayStubEmailService
{
    private const int MaxConcurrency = 5;
    private readonly string webhook = teamsWebhookOptions.Value.Accounting;

    public async Task Send(BulkPayStubEmailJob job, CancellationToken cancellationToken)
    {
        using var throttler = new SemaphoreSlim(MaxConcurrency);
        var tasks = job.PayStubIds.Select(payStubId => SendPayStubEmail(payStubId, throttler, cancellationToken));
        var results = await Task.WhenAll(tasks);
        await NotifyTeams(job, results);
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
