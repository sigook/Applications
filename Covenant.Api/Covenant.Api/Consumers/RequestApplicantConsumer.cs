using Azure.Messaging.ServiceBus;
using Covenant.Common.Configuration;
using Covenant.Common.Entities.Requests;
using Covenant.Common.Interfaces;
using Covenant.Core.BL.Interfaces.Candidates;
using Covenant.Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace Covenant.Api.Consumers;

public class RequestApplicantConsumer(ISigookBusClient client, IServiceScopeFactory serviceScopeFactory, IOptions<ServiceBusConfiguration> options) : IAzureServiceBusConsumer
{
    private readonly ServiceBusConfiguration serviceBusConfiguration = options.Value;

    public async Task OnInit()
    {
        await client.CreateProcessorAsync(serviceBusConfiguration.CreateApplicantTopic, TopicSubscription.RequestApplicantNotification, ProcessAsync);
    }

    private async Task ProcessAsync(ProcessMessageEventArgs args)
    {
        var applicant = args.Message.Body.ToObjectFromJson<RequestApplicant>();
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<ICandidateApplicationService>().RegisterApplicant(applicant);
        await args.CompleteMessageAsync(args.Message);
    }
}
