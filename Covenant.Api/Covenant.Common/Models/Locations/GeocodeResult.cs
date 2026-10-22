using System.Text.Json.Serialization;

namespace Covenant.Common.Models.Locations
{
    public class GeocodeResult
    {
        [JsonPropertyName("formatted_address")]
        public string FormattedAddress { get; set; }
        public GeocodeGeometry Geometry { get; set; }
    }
}