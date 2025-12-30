using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Agency;
using Covenant.Common.Enums;
using Covenant.Common.Models;
using Covenant.Common.Models.Agency;
using Covenant.Common.Models.Location;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Mappers;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;

namespace Covenant.Infrastructure.Repositories.Agency;

public class AgencyRepository : IAgencyRepository
{
    private readonly CovenantContext _context;
    private readonly FilesConfiguration filesConfiguration;

    public AgencyRepository(
        CovenantContext context,
        IOptions<FilesConfiguration> options)
    {
        _context = context;
        filesConfiguration = options.Value;
    }

    public async Task Create<T>(T entity) where T : class => await _context.Set<T>().AddAsync(entity);

    public void Update<T>(T entity) where T : class => _context.Set<T>().Update(entity);

    public async Task<Common.Entities.Agency.Agency> GetAgency(Guid id)
    {
        var query = _context.Agencies
            .Include(a => a.User)
            .Include(c => c.Logo)
            .Include(c => c.WsibGroup).ThenInclude(c => c.WsibGroup)
            .Include(c => c.Locations).ThenInclude(c => c.Location).ThenInclude(l => l.City).ThenInclude(c => c.Province).ThenInclude(p => p.Country)
            .Include(c => c.ContactInformation)
            .Where(c => c.Id == id);
        return await query.FirstOrDefaultAsync();
    }

    public async Task<AgencyModel> GetAgencyDetail(Guid id)
    {
        var query = _context.Agencies
            .Where(a => a.Id == id)
            .Select(AgencyExtensionsMapping.SelectAgency);
        var agency = await query.FirstOrDefaultAsync();
        if (agency.Logo != null)
        {
            agency.Logo.PathFile = $"{filesConfiguration.FilesPath}{agency.Logo.FileName}";
        }
        return agency;
    }

    public Task<bool> AgencyExists(Guid id) => _context.Agencies.AnyAsync(c => c.Id == id);

    public Task<List<LocationDetailModel>> GetLocations(Guid agencyId) =>
        _context.AgencyLocations.Where(w => w.AgencyId == agencyId)
            .Select(AgencyExtensionsMapping.SelectLocation).AsNoTracking().ToListAsync();

    public Task<LocationDetailModel> GetLocationDetail(Guid agencyId, Guid id) =>
        _context.AgencyLocations.Where(w => w.AgencyId == agencyId && w.LocationId == id)
            .Select(AgencyExtensionsMapping.SelectLocation).AsNoTracking().SingleOrDefaultAsync();

    public Task SaveChangesAsync() => _context.SaveChangesAsync();

    public async Task<IEnumerable<AgencyPersonnelModel>> GetAllPersonnel(Guid agencyId) =>
        await _context.AgencyPersonnel.Where(c => c.AgencyId == agencyId)
            .Select(s => new AgencyPersonnelModel
            {
                Id = s.Id,
                Name = s.Name,
                Email = s.User.Email
            }).ToListAsync();

    public Task<AgencyPersonnel> GetPersonnel(Guid id) =>
        _context.AgencyPersonnel.Where(c => c.Id == id)
            .Include(i => i.User)
            .SingleOrDefaultAsync();

    public Task<List<AgencyPersonnel>> GetPersonnelByUserId(Guid id) =>
        _context.AgencyPersonnel.Where(c => c.UserId == id)
            .Include(i => i.User).ToListAsync();

    public Task<AgencyPersonnelModel> GetPersonnel(Guid agencyId, Guid id) =>
        _context.AgencyPersonnel.Where(c => c.AgencyId == agencyId && c.Id == id)
            .Select(s => new AgencyPersonnelModel
            {
                Id = s.Id,
                Name = s.Name,
                Email = s.User.Email
            }).SingleOrDefaultAsync();

    public Task DeletePersonnel(AgencyPersonnel entity)
    {
        _context.AgencyPersonnel.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<PersonnelAgencyModel>> GetPersonnelAgency(Guid userId)
    {
        var result = _context.AgencyPersonnel
            .Where(c => c.UserId == userId)
            .Select(s => new PersonnelAgencyModel
            {
                Id = s.Id,
                Name = s.Agency.FullName,
                Email = s.Agency.User.Email,
                AgencyId = s.Agency.Id,
                IsPrimary = s.IsPrimary,
                Logo = s.Agency.Logo == null ? null : $"{filesConfiguration.FilesPath}{s.Agency.Logo.FileName}"
            });
        return await result.ToListAsync();
    }

    public async Task<Location> GetBillingLocation(Guid agencyId)
    {
        var query = _context.AgencyLocations
            .Include(i => i.Location).ThenInclude(l => l.City).ThenInclude(i => i.Province).ThenInclude(i => i.Country)
            .Where(f => f.AgencyId == agencyId && f.IsBilling)
            .Select(s => s.Location);
        return await query.FirstOrDefaultAsync();
    }

    public Task<Guid> GetAgencyIdForUser(Guid userId) =>
        _context.AgencyPersonnel.Where(w => w.UserId == userId)
            .OrderByDescending(o => o.IsPrimary)
            .Select(s => s.AgencyId).FirstOrDefaultAsync();

    public async Task<List<Guid>> GetAgencyIdsForUser(Guid userId)
    {
        // Get the user's primary agency ID
        var agencyId = await GetAgencyIdForUser(userId);

        if (agencyId == Guid.Empty)
        {
            return new List<Guid>();
        }

        // Get the agency to check its type
        var agency = await _context.Agencies
            .Where(a => a.Id == agencyId)
            .Select(a => new { a.Id, a.AgencyType })
            .FirstOrDefaultAsync();

        if (agency == null)
        {
            return new List<Guid> { agencyId };
        }

        // If Master agency, include all child agencies
        if (agency.AgencyType == AgencyType.Master)
        {
            var childAgencyIds = await _context.Agencies
                .Where(a => a.AgencyParentId == agencyId)
                .Select(a => a.Id)
                .ToListAsync();

            // Return the master agency ID + all child IDs
            var allAgencyIds = new List<Guid> { agency.Id };
            allAgencyIds.AddRange(childAgencyIds);
            return allAgencyIds;
        }

        // For Regular/BusinessPartner agencies, return only their own ID
        return new List<Guid> { agency.Id };
    }

    public async Task<Common.Entities.Agency.Agency> GetAgencyMasterByLocation(CityModel city)
    {
        var country = await _context.City.AsNoTracking()
            .Include(c => c.Province).ThenInclude(p => p.Country)
            .Where(c => c.Id == city.Id)
            .Select(c => c.Province.Country)
            .FirstOrDefaultAsync();
        var agencyLocations = _context.AgencyLocations
            .Include(al => al.Location)
            .ThenInclude(l => l.City)
            .ThenInclude(c => c.Province)
            .ThenInclude(p => p.Country)
            .Include(al => al.Agency)
            .Where(al => al.Agency.AgencyType == AgencyType.Master)
            .Where(al => al.Location.City.Province.CountryId == country.Id)
            .ToList();
        var agencyLocation = agencyLocations.FirstOrDefault();
        return agencyLocation.Agency;
    }

    public async Task<Common.Entities.Agency.Agency> GetAgencyMasterByCountry(Guid countryId)
    {
        var agencyLocations = await _context.AgencyLocations
            .Include(al => al.Location)
            .ThenInclude(l => l.City)
            .ThenInclude(c => c.Province)
            .ThenInclude(p => p.Country)
            .Include(al => al.Agency)
            .Where(al => al.Agency.AgencyType == AgencyType.Master)
            .Where(al => al.Location.City.Province.CountryId == countryId)
            .ToListAsync();
        var agencyLocation = agencyLocations.FirstOrDefault();
        return agencyLocation.Agency;
    }

    public async Task<PaginatedList<AgencyModel>> GetAgencies(Guid agencyId, GetAgenciesFilter filter)
    {
        var query = GetAllAgencies(agencyId, filter);
        return await query.ToPaginatedList(filter);
    }

    public IEnumerable<AgencyModel> GetAllAgencies(Guid agencyId, GetAgenciesFilter filter)
    {
        var agencies = _context.Agencies
            .Include(a => a.User)
            .Include(a => a.Locations).ThenInclude(l => l.Location).ThenInclude(l => l.City).ThenInclude(c => c.Province).ThenInclude(p => p.Country)
            .Where(a => a.AgencyParentId == agencyId);

        var query = agencies.Select(a => new AgencyModel
        {
            Id = a.Id,
            FullName = a.FullName,
            PhonePrincipal = a.PhonePrincipal,
            PhonePrincipalExt = a.PhonePrincipalExt,
            Email = a.User.Email,
            Locations = a.Locations.Select(l => new LocationDetailModel
            {
                Id = l.LocationId,
                Address = l.Location.Address,
                PostalCode = l.Location.PostalCode,
                City = new CityModel
                {
                    Id = l.Location.City.Id,
                    Value = l.Location.City.Value,
                    Code = l.Location.City.Code,
                    Province = new ProvinceModel
                    {
                        Id = l.Location.City.Province.Id,
                        Value = l.Location.City.Province.Value,
                        Code = l.Location.City.Province.Code,
                        Country = new CountryModel
                        {
                            Id = l.Location.City.Province.Country.Id,
                            Value = l.Location.City.Province.Country.Value,
                            Code = l.Location.City.Province.Country.Code
                        }
                    }
                },
                IsBilling = l.IsBilling
            }),
            AgencyType = a.AgencyType
        });

        var predicate = ApplyFilterAgencies(agencyId, filter);
        query = query.Where(predicate);
        query = ApplySortAgencies(query, filter);
        return query;
    }

    private Expression<Func<AgencyModel, bool>> ApplyFilterAgencies(Guid agencyId, GetAgenciesFilter filter)
    {
        var predicate = PredicateBuilder.New<AgencyModel>(true);
        if (!string.IsNullOrWhiteSpace(filter.FullName))
        {
            var fullName = filter.FullName.ToLower();
            predicate = predicate.And(a => a.FullName.ToLower().Contains(fullName));
        }
        if (!string.IsNullOrWhiteSpace(filter.Email))
        {
            var email = filter.Email.ToLower();
            predicate = predicate.And(a => a.Email.ToLower().Contains(email));
        }
        if (filter.AgencyTypes?.Any() == true)
        {
            predicate = predicate.And(a => filter.AgencyTypes.Contains(a.AgencyType));
        }
        return predicate;
    }

    private IQueryable<AgencyModel> ApplySortAgencies(IQueryable<AgencyModel> query, GetAgenciesFilter filter)
    {
        switch (filter.SortBy)
        {
            case GetAgenciesSortBy.FullName:
                query = query.AddOrderBy(filter, a => a.FullName);
                break;
            case GetAgenciesSortBy.Email:
                query = query.AddOrderBy(filter, a => a.Email);
                break;
        }

        return query;
    }
}