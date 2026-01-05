using Newtonsoft.Json;

namespace Covenant.Common.Models
{
    public class PotentialAction
    {
        [JsonProperty("@type")]
        public string Type { get; set; } = "OpenUri";

        [JsonProperty("name")]
        public string Name { get; set; } = "Go To sigook.com";

        [JsonProperty("targets")]
        public IEnumerable<Target> Targets { get; set; } = new List<Target>();
    }
}
