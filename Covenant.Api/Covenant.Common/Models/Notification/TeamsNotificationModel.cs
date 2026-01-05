using Newtonsoft.Json;

namespace Covenant.Common.Models.Notification
{
    public class TeamsNotificationModel
    {
        public TeamsNotificationModel(string title, string text)
        {
            Title = title;
            Text = text;
        }

        [JsonProperty("themeColor")]
        public string ThemeColor { get; private set; } = "0072C6";

        [JsonProperty("title")]
        public string Title { get; private set; }

        [JsonProperty("text")]
        public string Text { get; private set; }

        [JsonProperty("potentialAction")]
        public IEnumerable<PotentialAction> PotentialAction { get; set; } = new List<PotentialAction>();

        public static TeamsNotificationModel CreateSuccess(string title, string text) =>
            new(title, text);

        public static TeamsNotificationModel CreateError(string title, string text) =>
            new(title, text) { ThemeColor = "FF0000" };

        public static TeamsNotificationModel CreateWarning(string title, string text) =>
            new(title, text) { ThemeColor = "FFC200" };
    }
}