using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Mime;

namespace Covenant.Infrastructure.Services
{
    public class SigookBusClient : ServiceBusClient
    {
        private readonly SigookBusAdministrationClient administrationClient;
        private readonly ILogger<SigookBusClient> logger;

        public SigookBusClient(string connectionString, SigookBusAdministrationClient administrationClient, ILogger<SigookBusClient> logger)
            : base(connectionString)
        {
            this.administrationClient = administrationClient;
            this.logger = logger;
        }

        public async Task CreateProcessorAsync(string topic, string subscription, Func<ProcessMessageEventArgs, Task> processHandler, Func<ProcessErrorEventArgs, Task> failHandler = null)
        {
            if (!await ExistsSenderAsync(topic))
            {
                throw new InvalidOperationException($"Topic {topic} does not exist");
            }
            var processor = CreateProcessor(topic, subscription, new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false
            });
            processor.ProcessMessageAsync += processHandler;
            processor.ProcessErrorAsync += failHandler ?? OnFailDefault;
            await processor.StartProcessingAsync();
        }

        public async Task CreateProcessorAsync(string queue, Func<ProcessMessageEventArgs, Task> processHandler, Func<ProcessErrorEventArgs, Task> failHandler = null)
        {
            if (!await ExistsSenderAsync(queue))
            {
                throw new InvalidOperationException($"Queue {queue} does not exist");
            }
            var processor = CreateProcessor(queue, new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false
            });
            processor.ProcessMessageAsync += processHandler;
            processor.ProcessErrorAsync += failHandler ?? OnFailDefault;
            await processor.StartProcessingAsync();
        }

        public async Task SendMessageAsync(ServiceBusMessage message, string queueOrTopic)
        {
            await using var sender = CreateSender(queueOrTopic);
            await sender.SendMessageAsync(message);
        }

        public async Task SendMessageAsync<T>(T message, string queueOrTopic)
        {
            var busMessage = new ServiceBusMessage(JsonConvert.SerializeObject(message))
            {
                ContentType = MediaTypeNames.Application.Json,
            };
            await SendMessageAsync(busMessage, queueOrTopic);
        }

        private async Task OnFailDefault(ProcessErrorEventArgs args)
        {
            logger.LogInformation(args.Exception.ToString());
            await Task.CompletedTask;
        }

        private async ValueTask<bool> ExistsSenderAsync(string queueOrTopicName)
        {
            if (await administrationClient.QueueExistsAsync(queueOrTopicName) || await administrationClient.TopicExistsAsync(queueOrTopicName))
            {
                return true;
            }
            return false;
        }
    }
}
