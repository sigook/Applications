using Newtonsoft.Json;

namespace Covenant.Common.Models.Location
{
    public class GeocodeResult
    {
        [JsonProperty("formatted_address")]
        public string FormattedAddress { get; set; }
        public GeocodeGeometry Geometry { get; set; }
    }
}