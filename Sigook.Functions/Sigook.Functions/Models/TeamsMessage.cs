using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sigook.Functions.Models
{
	public class TeamsMessage
	{
		public TeamsMessage(string title,string text)
		{
			Title = title;
			Text = text;
		}
		[JsonPropertyName("@context")] public string Context { get; } = "https://schema.org/extensions";

		[JsonPropertyName("@type")] public string Type { get; } = "MessageCard";

		[JsonPropertyName("themeColor")] public string ThemeColor { get; set; } = "0072C6";

		[JsonPropertyName("title")] public string Title { get; set; }

		[JsonPropertyName("text")] public string Text { get; set; }

		[JsonPropertyName("potentialAction")] public IEnumerable<PotentialAction> PotentialAction { get; set; } = new List<PotentialAction>();

		public static TeamsMessage CreateSuccess(string title, string text) =>
			new TeamsMessage(title,text);

		public static TeamsMessage CreateError(string title, string text) =>
			new TeamsMessage(title, text) {ThemeColor = "FF0000"};
	}

	public  class PotentialAction
	{
		[JsonPropertyName("@type")] public string Type { get; set; } = "OpenUri";

		[JsonPropertyName("name")] public string Name { get; set; } = "Go To sigook.com";

		[JsonPropertyName("targets")] public IEnumerable<Target> Targets { get; set; } = new List<Target>();
	}

	public class Target
	{
		[JsonPropertyName("os")] public string Os { get; set; } = "default";

		[JsonPropertyName("uri")] public string Uri { get; set; } = "https://www.sigook.com";
	}
}