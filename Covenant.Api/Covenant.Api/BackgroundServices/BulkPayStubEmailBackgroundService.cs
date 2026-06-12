using Covenant.Common.Configuration;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Core.BL.Interfaces;
using Covenant.Common.Models.Notification;
using Microsoft.Extensions.Options;

namespace Covenant.Api.BackgroundServices;

public class BulkPayStubEmailBackgroundService : BackgroundService
{
    private const int MaxConcurrency = 5;
    private readonly IBulkPayStubEmailQueue _queue;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BulkPayStubEmailBackgroundService> _logger;

    public BulkPayStubEmailBackgroundService(
        IBulkPayStubEmailQueue queue,
        IServiceProvider serviceProvider,
        ILogger<BulkPayStubEmailBackgroundService> logger)
    {
        _queue = queue;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var job = await _queue.Dequeue(stoppingToken);
                await ProcessJob(job, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to process bulk pay stub email job");
            }
        }
    }

    private async Task ProcessJob(BulkPayStubEmailJob job, CancellationToken stoppingToken)
    {
        using var throttler = new SemaphoreSlim(MaxConcurrency);
        var tasks = job.PayStubIds.Select(payStubId => SendPayStubEmail(payStubId, throttler, stoppingToken));
        var results = await Task.WhenAll(tasks);
        await NotifyTeams(job, results);
    }

    private async Task<PayStubEmailResult> SendPayStubEmail(Guid payStubId, SemaphoreSlim throttler, CancellationToken stoppingToken)
    {
        await throttler.WaitAsync(stoppingToken);
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var payStubService = scope.ServiceProvider.GetRequiredService<IPayStubService>();
            return await payStubService.SendPayStubEmail(payStubId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to send pay stub {PayStubId}", payStubId);
            return PayStubEmailResult.Failed(payStubId);
        }
        finally
        {
            throttler.Release();
        }
    }

    private async Task NotifyTeams(BulkPayStubEmailJob job, IReadOnlyCollection<PayStubEmailResult> results)
    {
        using var scope = _serviceProvider.CreateScope();
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
