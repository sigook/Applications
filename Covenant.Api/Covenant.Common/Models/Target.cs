using System.Text.Json.Serialization;

namespace Covenant.Common.Models
{
    public class Target
    {
        [JsonPropertyName("os")]
        public string Os { get; set; } = "default";

        [JsonPropertyName("uri")]
        public string Uri { get; set; } = "https://covenant.sigook.ca";
    }
}
