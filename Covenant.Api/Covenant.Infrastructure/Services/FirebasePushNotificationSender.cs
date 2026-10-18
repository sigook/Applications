using Covenant.Common.Configuration;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Notification;
using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Covenant.Infrastructure.Services;

public class FirebasePushNotificationSender(
    IOptions<PushNotificationConfiguration> options,
    ILogger<FirebasePushNotificationSender> logger) : IPushNotificationSender
{
    public async Task Send(NotificationModel model)
    {
        try
        {
            var defaultApp = FirebaseApp.DefaultInstance;
            if (defaultApp is null)
            {
                var json = JsonSerializer.Serialize(options.Value);
                defaultApp = FirebaseApp.Create(new AppOptions { Credential = GoogleCredential.FromJson(json) });
            }
            var message = new Message
            {
                Notification = new Notification { Title = model.Title, Body = model.Body },
                Topic = model.Topic,
                Data = model.Data
            };
            await FirebaseMessaging.GetMessaging(defaultApp).SendAsync(message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Push notification failed. Topic={Topic} Title={Title}", model.Topic, model.Title);
        }
    }
}
