using Covenant.Common.Entities;
using Covenant.Common.Models;
using Covenant.Common.Repositories;
using Covenant.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly CovenantContext _context;

        public LocationRepository(CovenantContext context) => _context = context;

        public async Task<IEnumerable<Location>> GetLocationWithoutGeocode() =>
            await _context.Location
            .Include(l => l.City)
            .ThenInclude(c => c.Province)
            .ThenInclude(c => c.Country)
            .Where(c => c.Latitude == null || c.Longitude == null)
            .ToListAsync();

        public Task UpdateLocation(Location location)
        {
            _context.Location.Update(location);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        public async Task<Location> GetLocationById(Guid id)
        {
            var location = _context.Location
                .Include(l => l.City)
                .ThenInclude(c => c.Province)
                .ThenInclude(p => p.Country)
                .Where(l => l.Id == id);
            return await location.FirstOrDefaultAsync();
        }

        public async Task<LocationTaxModel> GetLocationTax(Guid locationId)
        {
            var locationTax = await _context.LocationTaxes
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
            var locationTax = await _context.LocationTaxes
                .FirstOrDefaultAsync(lt => lt.LocationId == locationId);
            if (locationTax is null)
            {
                _context.LocationTaxes.Add(new LocationTax
                {
                    LocationId = locationId,
                    Tax1 = tax1
                });
            }
            else
            {
                locationTax.Tax1 = tax1;
            }
        }
    }
}