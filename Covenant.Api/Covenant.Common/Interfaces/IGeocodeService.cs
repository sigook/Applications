using Covenant.Common.Models.Location;

namespace Covenant.Common.Interfaces
{
    public interface IGeocodeService
    {
        Task<GeocodeGeometryLocation> GetLocationGeocode(string address);
    }
}