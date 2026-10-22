using Covenant.Common.Entities;
using Covenant.Common.Models.Locations;
using Covenant.Common.Repositories;
using Covenant.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Repositories;

public class LocationRepository(CovenantContext context) : BaseRepository<CovenantContext>(context), ILocationRepository
{
    public async Task<IEnumerable<Location>> GetLocationWithoutGeocode() =>
        await Context.Locations
        .Include(l => l.City)
        .ThenInclude(c => c.Province)
        .ThenInclude(c => c.Country)
        .Where(c => c.Latitude == null || c.Longitude == null)
        .ToListAsync();

    public async Task<Location> GetLocationById(Guid id)
    {
        var location = Context.Locations
            .Include(l => l.City)
            .ThenInclude(c => c.Province)
            .ThenInclude(p => p.Country)
            .Where(l => l.Id == id);
        return await location.FirstOrDefaultAsync();
    }

    public async Task<LocationTaxModel> GetLocationTax(Guid locationId)
    {
        var locationTax = await Context.LocationTaxes
            .Where(lt => lt.LocationId == locationId)
            .Select(lt => new LocationTaxModel
            {
                Tax1 = lt.Tax1
            })
            .FirstOrDefaultAsync();
        return locationTax;
    }

    public async Task UpsertLocationTax(Guid locationId, decimal tax1)
    {
        var locationTax = await Context.LocationTaxes
            .FirstOrDefaultAsync(lt => lt.LocationId == locationId);
        if (locationTax is null)
        {
            Context.LocationTaxes.Add(new LocationTax(locationId, tax1));
        }
        else
        {
            locationTax.UpdateTax(tax1);
        }
    }
}
