using Newtonsoft.Json;

namespace Covenant.IdentityServer.Services.Models
{
	public class TeamsNotificationModel
	{
		[JsonProperty("themeColor")]public string ThemeColor { get; private set; }="0072C6";
		[JsonProperty("title")] public string Title { get; private set; }
		[JsonProperty("text")] public string Text { get; private set; }
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