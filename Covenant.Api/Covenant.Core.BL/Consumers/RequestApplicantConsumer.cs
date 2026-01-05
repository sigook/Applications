using Azure.Messaging.ServiceBus;
using Covenant.Common.Configuration;
using Covenant.Common.Entities.Request;
using Covenant.Common.Interfaces;
using Covenant.Common.Repositories.Candidate;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Repositories.Worker;
using Covenant.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Covenant.Core.BL.Consumers
{
    public class RequestApplicantConsumer : IAzureServiceBusConsumer
    {
        private readonly SigookBusClient client;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ServiceBusConfiguration serviceBusConfiguration;

        public RequestApplicantConsumer(SigookBusClient client, IServiceScopeFactory serviceScopeFactory, IOptions<ServiceBusConfiguration> options)
        {
            this.client = client;
            this.serviceScopeFactory = serviceScopeFactory;
            serviceBusConfiguration = options.Value;
        }

        public async Task OnInit()
        {
            await client.CreateProcessorAsync(serviceBusConfiguration.CreateApplicantTopic, TopicSubscription.RequestApplicantNotification, NotifyCandidateCreation);
        }

        public async Task NotifyCandidateCreation(ProcessMessageEventArgs args)
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var requestRepository = scope.ServiceProvider.GetService<IRequestRepository>();
            var message = args.Message.Body.ToObjectFromJson<RequestApplicant>();
            var candidateAppliedBefore = await requestRepository.GetRequestApplicant(ra => ra.RequestId == message.RequestId && ra.WorkerProfileId == message.WorkerProfileId && ra.CandidateId == message.CandidateId);
            if (candidateAppliedBefore == null)
            {
                await requestRepository.Create(message);
                await requestRepository.SaveChangesAsync();
            }
            await args.CompleteMessageAsync(args.Message);
        }
    }
}
