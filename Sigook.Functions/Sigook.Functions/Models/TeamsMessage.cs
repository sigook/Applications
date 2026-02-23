using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sigook.Functions.Models
{
	public class TeamsMessage
	{
		public TeamsMessage(string title,string text)
		{
			Title = title;
			Text = text;
		}
		[JsonProperty("@context")] public string Context { get; } = "https://schema.org/extensions";

		[JsonProperty("@type")] public string Type { get; } = "MessageCard";

		[JsonProperty("themeColor")] public string ThemeColor { get; set; } = "0072C6";

		[JsonProperty("title")] public string Title { get; set; }

		[JsonProperty("text")] public string Text { get; set; }

		[JsonProperty("potentialAction")] public IEnumerable<PotentialAction> PotentialAction { get; set; } = new List<PotentialAction>();

		public static TeamsMessage CreateSuccess(string title, string text) => 
			new TeamsMessage(title,text);

		public static TeamsMessage CreateError(string title, string text) => 
			new TeamsMessage(title, text) {ThemeColor = "FF0000"};
	}

	public  class PotentialAction
	{
		[JsonProperty("@type")] public string Type { get; set; } = "OpenUri";

		[JsonProperty("name")] public string Name { get; set; } = "Go To sigook.com";

		[JsonProperty("targets")] public IEnumerable<Target> Targets { get; set; } = new List<Target>();
	}

	public class Target
	{
		[JsonProperty("os")] public string Os { get; set; } = "default";

		[JsonProperty("uri")] public string Uri { get; set; } = "https://www.sigook.com";
	}
}