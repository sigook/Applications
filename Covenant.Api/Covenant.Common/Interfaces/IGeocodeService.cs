using Covenant.Common.Models.Locations;

namespace Covenant.Common.Interfaces
{
    public interface IGeocodeService
    {
        Task<GeocodeGeometryLocation> GetLocationGeocode(string address);
    }
}