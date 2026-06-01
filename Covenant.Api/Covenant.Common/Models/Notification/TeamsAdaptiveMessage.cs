using System.Text.Json.Serialization;

namespace Covenant.Common.Models.Notification
{
    public class TeamsAdaptiveMessage
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "message";

        [JsonPropertyName("attachments")]
        public IEnumerable<TeamsAdaptiveAttachment> Attachments { get; set; } = new List<TeamsAdaptiveAttachment>();

        public static TeamsAdaptiveMessage FromNotification(TeamsNotificationModel notification)
        {
            var items = new List<object>
            {
                new AdaptiveTextBlock { Text = notification.Title, Weight = "Bolder", Size = "Large", Wrap = true },
                new AdaptiveTextBlock { Text = notification.Text, Wrap = true }
            };

            var actions = notification.PotentialAction
                .SelectMany(action => action.Targets.Select(target => new AdaptiveOpenUrlAction
                {
                    Title = action.Name,
                    Url = target.Uri
                }))
                .Cast<object>()
                .ToList();

            var card = new AdaptiveCard
            {
                Body =
                [
                    new AdaptiveContainer
                    {
                        Style = MapStyle(notification.ThemeColor),
                        Bleed = true,
                        Items = items
                    }
                ],
                Actions = actions
            };

            return new TeamsAdaptiveMessage
            {
                Attachments = [new TeamsAdaptiveAttachment { Content = card }]
            };
        }

        private static string MapStyle(string themeColor) => themeColor switch
        {
            "FF0000" => "attention",
            "FFC200" => "warning",
            _ => "good"
        };
    }

    public class TeamsAdaptiveAttachment
    {
        [JsonPropertyName("contentType")]
        public string ContentType { get; set; } = "application/vnd.microsoft.card.adaptive";

        [JsonPropertyName("content")]
        public AdaptiveCard Content { get; set; }
    }

    public class AdaptiveCard
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "AdaptiveCard";

        [JsonPropertyName("$schema")]
        public string Schema { get; set; } = "http://adaptivecards.io/schemas/adaptive-card.json";

        [JsonPropertyName("version")]
        public string Version { get; set; } = "1.4";

        [JsonPropertyName("body")]
        public IEnumerable<object> Body { get; set; } = new List<object>();

        [JsonPropertyName("actions")]
        public IEnumerable<object> Actions { get; set; } = new List<object>();
    }

    public class AdaptiveContainer
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "Container";

        [JsonPropertyName("style")]
        public string Style { get; set; }

        [JsonPropertyName("bleed")]
        public bool Bleed { get; set; }

        [JsonPropertyName("items")]
        public IEnumerable<object> Items { get; set; } = new List<object>();
    }

    public class AdaptiveTextBlock
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "TextBlock";

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("weight")]
        public string Weight { get; set; }

        [JsonPropertyName("size")]
        public string Size { get; set; }

        [JsonPropertyName("wrap")]
        public bool Wrap { get; set; }
    }

    public class AdaptiveOpenUrlAction
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "Action.OpenUrl";

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
