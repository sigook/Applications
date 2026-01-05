using Covenant.Common.Entities;
using Covenant.Common.Models;

namespace Covenant.Common.Repositories
{
    public interface ILocationRepository
    {
        Task<Location> GetLocationById(Guid id);
        Task<ProvinceTaxModel> GetProvinceSalesTax(Guid provinceId);
        Task<IEnumerable<Location>> GetLocationWithoutGeocode();
        Task UpdateLocation(Location location);
        Task SaveChangesAsync();
    }
}