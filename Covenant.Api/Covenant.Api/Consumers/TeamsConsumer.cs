using Azure.Messaging.ServiceBus;
using Covenant.Common.Configuration;
using Covenant.Common.Constants;
using Covenant.Common.Entities.Request;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Candidate;
using Covenant.Core.BL.Interfaces.Candidates;
using Covenant.Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace Covenant.Api.Consumers;

public class TeamsConsumer(ISigookBusClient client, IServiceScopeFactory serviceScopeFactory, IOptions<ServiceBusConfiguration> options) : IAzureServiceBusConsumer
{
    private readonly ServiceBusConfiguration serviceBusConfiguration = options.Value;

    public async Task OnInit()
    {
        await client.CreateProcessorAsync(serviceBusConfiguration.CreateApplicantTopic, TopicSubscription.TeamsNotification, ProcessAsync);
    }

    private async Task ProcessAsync(ProcessMessageEventArgs args)
    {
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var candidateApplicationService = scope.ServiceProvider.GetRequiredService<ICandidateApplicationService>();
        var properties = args.Message.ApplicationProperties;
        if (properties.TryGetValue(ServiceBusSqlConstants.ErrorCandidate, out var error))
        {
            await candidateApplicationService.NotifyFailure(error.ToString());
        }
        else if ((bool)properties[ServiceBusSqlConstants.RequestApplication])
        {
            await candidateApplicationService.NotifyApplication(args.Message.Body.ToObjectFromJson<RequestApplicant>());
        }
        else
        {
            await candidateApplicationService.NotifyCreation(args.Message.Body.ToObjectFromJson<CandidateCreateModel>());
        }
        await args.CompleteMessageAsync(args.Message);
    }
}
