using System.Text.Json.Serialization;

namespace Covenant.Common.Models
{
    public class PotentialAction
    {
        [JsonPropertyName("@type")]
        public string Type { get; set; } = "OpenUri";

        [JsonPropertyName("name")]
        public string Name { get; set; } = "Go To sigook.com";

        [JsonPropertyName("targets")]
        public IEnumerable<Target> Targets { get; set; } = new List<Target>();
    }
}
