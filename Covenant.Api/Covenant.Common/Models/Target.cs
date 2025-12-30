using Newtonsoft.Json;

namespace Covenant.Common.Models
{
    public class Target
    {
        [JsonProperty("os")]
        public string Os { get; set; } = "default";

        [JsonProperty("uri")]
        public string Uri { get; set; } = "https://covenant.sigook.ca";
    }
}
