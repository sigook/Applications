using Azure.Messaging.ServiceBus;
using Covenant.Common.Configuration;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Core.BL.Interfaces.Accounting.PayStubs;
using Covenant.Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace Covenant.Api.Consumers;

public class BulkPayStubEmailConsumer(ISigookBusClient client, IServiceScopeFactory serviceScopeFactory, IOptions<ServiceBusConfiguration> options) : IAzureServiceBusConsumer
{
    private readonly ServiceBusConfiguration serviceBusConfiguration = options.Value;

    public async Task OnInit()
    {
        await client.CreateProcessorAsync(serviceBusConfiguration.BulkPayStubEmailQueue, ProcessAsync);
    }

    private async Task ProcessAsync(ProcessMessageEventArgs args)
    {
        var job = args.Message.Body.ToObjectFromJson<BulkPayStubEmailJob>();
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<IBulkPayStubEmailService>().Send(job, args.CancellationToken);
        await args.CompleteMessageAsync(args.Message);
    }
}
