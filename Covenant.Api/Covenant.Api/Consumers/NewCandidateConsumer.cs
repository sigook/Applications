using Azure.Messaging.ServiceBus;
using Covenant.Common.Configuration;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Website;
using Covenant.Core.BL.Interfaces.Candidates;
using Covenant.Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace Covenant.Api.Consumers;

public class NewCandidateConsumer(ISigookBusClient client, IServiceScopeFactory serviceScopeFactory, IOptions<ServiceBusConfiguration> options) : IAzureServiceBusConsumer
{
    private readonly ServiceBusConfiguration serviceBusConfiguration = options.Value;

    public async Task OnInit()
    {
        await client.CreateProcessorAsync(serviceBusConfiguration.ValidateCandidateQueue, ProcessAsync);
    }

    private async Task ProcessAsync(ProcessMessageEventArgs args)
    {
        var message = args.Message.Body.ToObjectFromJson<CandidateViewModel>();
        if (message is not null)
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            await scope.ServiceProvider.GetRequiredService<ICandidateApplicationService>().ProcessApplication(message);
        }
        await args.CompleteMessageAsync(args.Message);
    }
}
