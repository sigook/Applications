using Covenant.Common.Configuration;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Notification;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Covenant.Infrastructure.Services
{
    public class PushNotifications : IPushNotifications
    {
        private readonly PushNotificationConfiguration pushNotificationConfiguration;

        public PushNotifications(IOptions<PushNotificationConfiguration> options)
        {
            pushNotificationConfiguration = options.Value;
        }

        public async Task SendNotification(NotificationModel model)
        {
            var defaultApp = FirebaseApp.DefaultInstance;
            if (defaultApp is null)
            {
                var json = JsonConvert.SerializeObject(pushNotificationConfiguration);
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
    }
}