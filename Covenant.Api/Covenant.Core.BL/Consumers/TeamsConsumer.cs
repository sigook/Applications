using Azure.Messaging.ServiceBus;
using Covenant.Common.Configuration;
using Covenant.Common.Constants;
using Covenant.Common.Entities.Request;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Candidate;
using Covenant.Common.Models.Notification;
using Covenant.Common.Repositories.Candidate;
using Covenant.Common.Repositories.Request;
using Covenant.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Covenant.Core.BL.Consumers
{
    public class TeamsConsumer : IAzureServiceBusConsumer
    {
        private readonly SigookBusClient client;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ServiceBusConfiguration serviceBusConfiguration;

        public TeamsConsumer(SigookBusClient client, IServiceScopeFactory serviceScopeFactory, IOptions<ServiceBusConfiguration> options)
        {
            this.client = client;
            this.serviceScopeFactory = serviceScopeFactory;
            serviceBusConfiguration = options.Value;
        }

        public async Task OnInit()
        {
            await client.CreateProcessorAsync(serviceBusConfiguration.CreateApplicantTopic, TopicSubscription.TeamsNotification, NotifyCandidateCreation);
        }

        private async Task NotifyCandidateCreation(ProcessMessageEventArgs args)
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var teamsAccountingNotification = scope.ServiceProvider.GetService<ITeamsService>();
            var options = scope.ServiceProvider.GetService<IOptions<TeamsWebhookConfiguration>>();
            var notification = default(TeamsNotificationModel);
            if (args.Message.ApplicationProperties.Any(k => k.Key == ServiceBusSqlConstants.ErrorCandidate))
            {
                notification = TeamsNotificationModel.CreateError("Candidate application", args.Message.ApplicationProperties[ServiceBusSqlConstants.ErrorCandidate].ToString());
            }
            else
            {
                var requestApplication = (bool)args.Message.ApplicationProperties[ServiceBusSqlConstants.RequestApplication];
                if (requestApplication)
                {
                    var candidateRepository = scope.ServiceProvider.GetService<ICandidateRepository>();
                    var requestRepository = scope.ServiceProvider.GetService<IRequestRepository>();
                    var message = args.Message.Body.ToObjectFromJson<RequestApplicant>();
                    var candidate = await candidateRepository.GetCandidate(c => c.Id == message.CandidateId.Value);
                    var request = await requestRepository.GetRequest(r => r.Id == message.RequestId);
                    notification = TeamsNotificationModel.CreateSuccess("Candidate application", $"Candidate {candidate.Name} applied to order number {request.NumberId}");
                }
                else
                {
                    var message = args.Message.Body.ToObjectFromJson<CandidateCreateModel>();
                    notification = TeamsNotificationModel.CreateSuccess("Candidate creation", $"Candidate {message.Name} has been created");
                }
            }
            await teamsAccountingNotification.SendNotification(options.Value.CandidateAndWorker, notification);
            await args.CompleteMessageAsync(args.Message);
        }
    }
}
