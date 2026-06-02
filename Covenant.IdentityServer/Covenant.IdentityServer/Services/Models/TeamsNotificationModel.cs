using System.Text.Json.Serialization;

namespace Covenant.IdentityServer.Services.Models
{
	public class TeamsNotificationModel
	{
		[JsonInclude]
		[JsonPropertyName("themeColor")] public string ThemeColor { get; private set; } = "0072C6";

		[JsonInclude]
		[JsonPropertyName("title")] public string Title { get; private set; }

		[JsonInclude]
		[JsonPropertyName("text")] public string Text { get; private set; }

		public TeamsNotificationModel(string title,string text)
		{
			Title = title;
			Text = text;
		}
		public static TeamsNotificationModel CreateSuccess(string title, string text) => 
			new TeamsNotificationModel(title,text);

		public static TeamsNotificationModel CreateError(string title, string text) => 
			new TeamsNotificationModel(title, text) {ThemeColor = "FF0000"};
		
		public static TeamsNotificationModel CreateWarning(string title, string text) => 
			new TeamsNotificationModel(title, text) {ThemeColor = "FFC200"};
	}
}