using Covenant.Common.Entities;
using Covenant.Common.Models;
using Covenant.Common.Repositories;
using Covenant.Infrastructure.Context;
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

        public async Task<ProvinceTaxModel> GetProvinceSalesTax(Guid provinceId)
        {
            var provinceTax = await _context.ProvinceTaxes
                .Where(pt => pt.ProvinceId == provinceId)
                .Select(pt => new ProvinceTaxModel
                {
                    Tax1 = pt.Tax1
                })
                .FirstOrDefaultAsync();
            return provinceTax;
        }
    }
}