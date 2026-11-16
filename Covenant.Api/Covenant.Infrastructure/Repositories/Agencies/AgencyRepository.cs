using Covenant.Common.Configuration;
using Covenant.Common.Entities.Agencies;
using Covenant.Common.Entities;
using Covenant.Common.Enums;
using Covenant.Common.Models.Agencies;
using Covenant.Common.Models.Locations;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Agencies;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Extensions;
using Covenant.Infrastructure.Mappers;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;

namespace Covenant.Infrastructure.Repositories.Agencies;

public class AgencyRepository(CovenantContext context, IOptions<FilesConfiguration> options) : BaseRepository<CovenantContext>(context), IAgencyRepository
{
    private readonly FilesConfiguration filesConfiguration = options.Value;

    public async Task<Agency> GetAgency(Guid id)
    {
        var query = Context.Agencies
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
        var query = Context.Agencies
            .Where(a => a.Id == id)
            .Select(AgencyExtensionsMapping.SelectAgency);
        var agency = await query.FirstOrDefaultAsync();
        if (agency.Logo != null)
        {
            agency.Logo.PathFile = $"{filesConfiguration.FilesPath}{agency.Logo.FileName}";
        }
        return agency;
    }

    public Task<bool> AgencyExists(Guid id) => Context.Agencies.AnyAsync(c => c.Id == id);

    public Task<List<LocationDetailModel>> GetLocations(Guid agencyId) =>
        Context.AgencyLocations.Where(w => w.AgencyId == agencyId)
            .Select(AgencyExtensionsMapping.SelectLocation).AsNoTracking().ToListAsync();

    public Task<LocationDetailModel> GetLocationDetail(Guid agencyId, Guid id) =>
        Context.AgencyLocations.Where(w => w.AgencyId == agencyId && w.LocationId == id)
            .Select(AgencyExtensionsMapping.SelectLocation).AsNoTracking().SingleOrDefaultAsync();

    public Task<List<AgencyPersonnelModel>> GetPersonnel(IEnumerable<Guid> userIds) =>
        Context.Users.Where(u => userIds.Contains(u.Id))
            .OrderBy(u => u.Name ?? u.Email)
            .ThenBy(u => u.Id)
            .Select(u => new AgencyPersonnelModel
            {
                Id = u.Id,
                UserId = u.Id,
                Name = u.Name,
                Email = u.Email
            }).AsNoTracking().ToListAsync();

    public async Task<bool> PersonnelHasRequests(Guid userId) =>
        await Context.RequestRecruiters.AnyAsync(r => r.RecruiterId == userId)
        || await Context.RequestComissions.AnyAsync(c => c.SalesRepresentativeId == userId);

    public Task<bool> PersonnelHasCompanies(Guid userId) =>
        Context.CompanyProfiles.AnyAsync(c => c.SalesRepresentativeId == userId);

    public async Task<Location> GetBillingLocation(Guid agencyId)
    {
        var agencyLocation = await Context.AgencyLocations
            .Include(i => i.Location).ThenInclude(l => l.City).ThenInclude(i => i.Province).ThenInclude(i => i.Country)
            .Where(f => f.AgencyId == agencyId && f.IsBilling)
            .FirstOrDefaultAsync();
        return agencyLocation?.Location;
    }

    public Task<List<AgencyOptionModel>> GetAgencyOptions(IEnumerable<Guid> agencyIds) =>
        Context.Agencies.Where(a => agencyIds.Contains(a.Id))
            .OrderBy(a => a.FullName)
            .ThenBy(a => a.Id)
            .Select(a => new AgencyOptionModel { Id = a.Id, Name = a.FullName })
            .ToListAsync();

    public async Task<Agency> GetAgencyMasterByLocation(CityModel city)
    {
        var country = await Context.Cities.AsNoTracking()
            .Where(c => c.Id == city.Id)
            .Select(c => c.Province.Country)
            .FirstOrDefaultAsync();
        if (country is null) return null;
        return await GetAgencyMasterByCountry(country.Id);
    }

    public async Task<Agency> GetAgencyMasterByCountry(Guid countryId)
    {
        var agencyLocation = await Context.AgencyLocations
            .Include(al => al.Location)
            .ThenInclude(l => l.City)
            .ThenInclude(c => c.Province)
            .ThenInclude(p => p.Country)
            .Include(al => al.Agency)
            .Where(al => al.Agency.AgencyType == AgencyType.Master)
            .Where(al => al.Location.City.Province.CountryId == countryId)
            .FirstOrDefaultAsync();
        return agencyLocation?.Agency;
    }

    public async Task<PaginatedList<AgencyModel>> GetAgencies(IEnumerable<Guid> agencyIds, GetAgenciesFilter filter)
    {
        var query = GetAllAgencies(agencyIds, filter);
        return await query.ToPaginatedList(filter);
    }

    public IQueryable<AgencyModel> GetAllAgencies(IEnumerable<Guid> agencyIds, GetAgenciesFilter filter)
    {
        var agencies = Context.Agencies
            .Include(a => a.User)
            .Include(a => a.Locations).ThenInclude(l => l.Location).ThenInclude(l => l.City).ThenInclude(c => c.Province).ThenInclude(p => p.Country)
            .Where(a => a.AgencyParentId.HasValue && agencyIds.Contains(a.AgencyParentId.Value));

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

        var predicate = ApplyFilterAgencies(agencyIds, filter);
        query = query.Where(predicate);
        query = ApplySortAgencies(query, filter);
        return query;
    }

    private Expression<Func<AgencyModel, bool>> ApplyFilterAgencies(IEnumerable<Guid> agencyIds, GetAgenciesFilter filter)
    {
        Expression<Func<AgencyModel, bool>> predicate = PredicateBuilder.New<AgencyModel>(true);
        predicate = predicate
            .AndSearch(filter.FullName, a => a.FullName)
            .AndSearch(filter.Email, a => a.Email);
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
            default:
                query = query.OrderBy(a => a.FullName);
                break;
        }

        return query;
    }

    public Task<UserAttendance> GetUserAttendance(Guid userId, DateTime date) =>
        Context.UserAttendances.FirstOrDefaultAsync(a => a.UserId == userId && a.Date == date.Date);

    public Task<UserAttendance> GetUserAttendanceOfUsers(Guid id, IEnumerable<Guid> userIds) =>
        UserAttendancesOf(userIds).FirstOrDefaultAsync(a => a.Id == id);

    public Task<List<UserAttendance>> GetUserAttendancesOfUsers(IEnumerable<Guid> userIds, DateTime date) =>
        UserAttendancesOf(userIds).Where(a => a.Date == date.Date).ToListAsync();

    public Task<List<UserAttendanceListModel>> GetUserAttendanceReport(IEnumerable<Guid> userIds, GetUserAttendanceReportFilter filter)
    {
        var query = UserAttendancesOf(userIds).Where(a => a.Date >= filter.From.Date && a.Date <= filter.To.Date);
        if (filter.UserId.HasValue) query = query.Where(a => a.UserId == filter.UserId.Value);
        return query
            .Select(a => new UserAttendanceListModel
            {
                Id = a.Id,
                UserId = a.UserId,
                Name = a.User.Name,
                Date = a.Date,
                ClockIn = a.ClockIn,
                ClockOut = a.ClockOut,
                LunchMinutes = a.LunchMinutes,
                IsEdited = a.EditedAt != null,
                EditedBy = a.EditedBy,
                EditedAt = a.EditedAt,
                EditReason = a.EditReason
            })
            .OrderBy(a => a.Name)
            .ThenBy(a => a.Date)
            .ToListAsync();
    }

    private IQueryable<UserAttendance> UserAttendancesOf(IEnumerable<Guid> userIds) =>
        Context.UserAttendances.Where(a => userIds.Contains(a.UserId));
}
