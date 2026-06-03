using Covenant.Common.Entities;
using Covenant.Common.Models;

namespace Covenant.Common.Repositories
{
    public interface ILocationRepository
    {
        Task<Location> GetLocationById(Guid id);
        Task<LocationTaxModel> GetLocationTax(Guid locationId);
        Task UpsertLocationTax(Guid locationId, decimal tax1);
        Task<IEnumerable<Location>> GetLocationWithoutGeocode();
        Task UpdateLocation(Location location);
        Task SaveChangesAsync();
    }
}