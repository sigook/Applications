using Covenant.Common.Configuration;
using Covenant.Common.Models.Notification;
using Covenant.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Covenant.Tests.Notifications;

public class PushNotificationPublisherTest
{
    [Fact]
    public async Task SendNotificationPublishesTheModelToThePushQueue()
    {
        var busClient = new Mock<ISigookBusClient>();
        var publisher = new PushNotificationPublisher(
            busClient.Object,
            Options.Create(new ServiceBusConfiguration { PushNotificationQueue = "PushNotificationTest" }));
        var model = NotificationModel.NewRequestNotification("Job Alert", "Welder CAD $30/h", Guid.NewGuid());

        await publisher.SendNotification(model);

        busClient.Verify(b => b.SendMessageAsync(model, "PushNotificationTest"), Times.Once);
        busClient.VerifyNoOtherCalls();
    }
}
