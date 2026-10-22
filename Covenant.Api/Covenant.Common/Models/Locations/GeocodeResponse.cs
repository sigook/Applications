namespace Covenant.Common.Models.Locations
{
    public class GeocodeResponse
    {
        public string Status { get; set; }
        public List<GeocodeResult> Results { get; set; } = new List<GeocodeResult>();
    }
}