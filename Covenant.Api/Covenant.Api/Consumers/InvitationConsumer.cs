using Azure.Messaging.ServiceBus;
using Covenant.Common.Configuration;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Requests;
using Covenant.Core.BL.Interfaces.Requests;
using Covenant.Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace Covenant.Api.Consumers;

public class InvitationConsumer(ISigookBusClient client, IServiceScopeFactory serviceScopeFactory, IOptions<ServiceBusConfiguration> options) : IAzureServiceBusConsumer
{
    private readonly ServiceBusConfiguration serviceBusConfiguration = options.Value;

    public async Task OnInit()
    {
        await client.CreateProcessorAsync(serviceBusConfiguration.InvitationQueue, ProcessAsync);
    }

    private async Task ProcessAsync(ProcessMessageEventArgs args)
    {
        var job = args.Message.Body.ToObjectFromJson<SendInvitationJob>();
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<IRequestInvitationService>().Send(job);
        await args.CompleteMessageAsync(args.Message);
    }
}
