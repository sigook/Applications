using Azure.Messaging.ServiceBus;
using Covenant.Common.Configuration;
using Covenant.Common.Interfaces;
using Covenant.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Covenant.Core.BL.Consumers
{
    public class EmailConsumer : IAzureServiceBusConsumer
    {
        private readonly SigookBusClient client;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ServiceBusConfiguration serviceBusConfiguration;

        public EmailConsumer(
            SigookBusClient client,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<ServiceBusConfiguration> options)
        {
            this.client = client;
            this.serviceScopeFactory = serviceScopeFactory;
            serviceBusConfiguration = options.Value;
        }

        public async Task OnInit()
        {
            await client.CreateProcessorAsync(serviceBusConfiguration.CreateApplicantTopic, TopicSubscription.EmailNotification, NotifyCandidateCreation);
        }

        private async Task NotifyCandidateCreation(ProcessMessageEventArgs args)
        {
            await Task.CompletedTask;
            using var scope = serviceScopeFactory.CreateScope();
        }
    }
}
