using Azure.Messaging.ServiceBus;

namespace Covenant.Infrastructure.Services;

public interface ISigookBusClient
{
    Task CreateProcessorAsync(string topic, string subscription, Func<ProcessMessageEventArgs, Task> processHandler, Func<ProcessErrorEventArgs, Task> failHandler = null);
    Task CreateProcessorAsync(string queue, Func<ProcessMessageEventArgs, Task> processHandler, Func<ProcessErrorEventArgs, Task> failHandler = null);
    Task SendMessageAsync(ServiceBusMessage message, string queueOrTopic);
    Task SendMessageAsync<T>(T message, string queueOrTopic);
}
