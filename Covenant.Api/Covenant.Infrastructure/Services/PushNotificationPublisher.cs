using Covenant.Common.Configuration;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Notification;
using Microsoft.Extensions.Options;

namespace Covenant.Infrastructure.Services;

public class PushNotificationPublisher(
    ISigookBusClient busClient,
    IOptions<ServiceBusConfiguration> options) : IPushNotifications
{
    public Task SendNotification(NotificationModel model) =>
        busClient.SendMessageAsync(model, options.Value.PushNotificationQueue);
}
