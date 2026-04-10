using Covenant.Common.Configuration;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Notification;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Covenant.Infrastructure.Services;

public class PushNotifications : IPushNotifications
{
    private readonly PushNotificationConfiguration pushNotificationConfiguration;
    private readonly ILogger<PushNotifications> logger;

    public PushNotifications(
        IOptions<PushNotificationConfiguration> options,
        ILogger<PushNotifications> logger)
    {
        pushNotificationConfiguration = options.Value;
        this.logger = logger;
    }

    public async Task SendNotification(NotificationModel model)
    {
        try
        {
            var defaultApp = FirebaseApp.DefaultInstance;
            if (defaultApp is null)
            {
                var json = JsonSerializer.Serialize(pushNotificationConfiguration);
                defaultApp = FirebaseApp.Create(new AppOptions { Credential = GoogleCredential.FromJson(json) });
            }
            var message = new Message
            {
                Notification = new Notification { Title = model.Title, Body = model.Body },
                Topic = model.Topic,
                Data = model.Data
            };
            var messaging = FirebaseMessaging.GetMessaging(defaultApp);
            await messaging.SendAsync(message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "PsuhNotification Error");
        }
    }
}