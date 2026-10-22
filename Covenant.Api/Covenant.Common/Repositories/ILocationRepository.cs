using Covenant.Common.Entities;
using Covenant.Common.Models.Locations;

namespace Covenant.Common.Repositories;

public interface ILocationRepository : IBaseRepository
{
    Task<Location> GetLocationById(Guid id);
    Task<LocationTaxModel> GetLocationTax(Guid locationId);
    Task UpsertLocationTax(Guid locationId, decimal tax1);
    Task<IEnumerable<Location>> GetLocationWithoutGeocode();
}
