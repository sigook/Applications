using Azure.Messaging.ServiceBus;
using Covenant.Common.Configuration;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Notification;
using Covenant.Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace Covenant.Core.BL.Consumers;

public class PushNotificationConsumer(
    ISigookBusClient client,
    IPushNotificationSender sender,
    IOptions<ServiceBusConfiguration> options) : IAzureServiceBusConsumer
{
    public async Task OnInit()
    {
        await client.CreateProcessorAsync(options.Value.PushNotificationQueue, ProcessAsync);
    }

    private async Task ProcessAsync(ProcessMessageEventArgs args)
    {
        var notification = args.Message.Body.ToObjectFromJson<NotificationModel>();
        if (notification is not null) await sender.Send(notification);
        await args.CompleteMessageAsync(args.Message);
    }
}
