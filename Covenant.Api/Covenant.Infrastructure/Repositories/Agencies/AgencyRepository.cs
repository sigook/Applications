using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Agency;
using Covenant.Common.Enums;
using Covenant.Common.Models;
using Covenant.Common.Models.Agency;
using Covenant.Common.Models.Location;
using Covenant.Common.Repositories.Agencies;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Mappers;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;

namespace Covenant.Infrastructure.Repositories.Agencies;

public class AgencyRepository(CovenantContext context, IOptions<FilesConfiguration> options) : BaseRepository<CovenantContext>(context), IAgencyRepository
{
    private readonly FilesConfiguration filesConfiguration = options.Value;

    public async Task<Common.Entities.Agency.Agency> GetAgency(Guid id)
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

    public async Task<IEnumerable<AgencyPersonnelModel>> GetAllPersonnel(Guid agencyId) =>
        await Context.AgencyPersonnel.Where(c => c.AgencyId == agencyId)
            .OrderBy(c => c.Name)
            .Select(s => new AgencyPersonnelModel
            {
                Id = s.Id,
                UserId = s.UserId,
                Name = s.Name,
                Email = s.User.Email
            }).ToListAsync();

    public Task<AgencyPersonnel> GetPersonnel(Guid id) =>
        Context.AgencyPersonnel.Where(c => c.Id == id)
            .Include(i => i.User)
            .SingleOrDefaultAsync();

    public Task<List<AgencyPersonnel>> GetPersonnelByUserId(Guid id) =>
        Context.AgencyPersonnel.Where(c => c.UserId == id)
            .Include(i => i.User).ToListAsync();

    public Task<List<AgencyPersonnel>> GetPersonnels(IEnumerable<Guid> ids) =>
        Context.AgencyPersonnel.Where(c => ids.Contains(c.Id))
            .Include(i => i.User).ToListAsync();

    public Task<AgencyPersonnelModel> GetPersonnel(Guid agencyId, Guid id) =>
        Context.AgencyPersonnel.Where(c => c.AgencyId == agencyId && c.Id == id)
            .Select(s => new AgencyPersonnelModel
            {
                Id = s.Id,
                UserId = s.UserId,
                Name = s.Name,
                Email = s.User.Email
            }).SingleOrDefaultAsync();

    public async Task<bool> PersonnelHasRequests(Guid personnelId) =>
        await Context.RequestRecruiters.AnyAsync(r => r.RecruiterId == personnelId)
        || await Context.RequestComissions.AnyAsync(c => c.AgencyPersonnelId == personnelId);

    public Task<bool> PersonnelHasCompanies(Guid personnelId) =>
        Context.CompanyProfiles.AnyAsync(c => c.SalesRepresentativeId == personnelId);

    public async Task<IEnumerable<PersonnelAgencyModel>> GetPersonnelAgency(Guid userId)
    {
        var result = Context.AgencyPersonnel
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
        var agencyLocation = await Context.AgencyLocations
            .Include(i => i.Location).ThenInclude(l => l.City).ThenInclude(i => i.Province).ThenInclude(i => i.Country)
            .Where(f => f.AgencyId == agencyId && f.IsBilling)
            .FirstOrDefaultAsync();
        return agencyLocation?.Location;
    }

    public async Task<AgencyUserContextModel> GetAgencyContextForUser(Guid userId)
    {
        var personnel = await Context.AgencyPersonnel.Where(p => p.UserId == userId)
            .OrderByDescending(p => p.IsPrimary)
            .Select(p => new { p.Id, p.AgencyId })
            .FirstOrDefaultAsync();
        if (personnel is null) return null;
        var agencies = await Context.Agencies
            .Where(a => a.Id == personnel.AgencyId || a.AgencyParentId == personnel.AgencyId)
            .Select(a => new { a.Id, a.AgencyType })
            .ToListAsync();
        var isMaster = agencies.Any(a => a.Id == personnel.AgencyId && a.AgencyType == AgencyType.Master);
        var childIds = isMaster ? agencies.Where(a => a.Id != personnel.AgencyId).Select(a => a.Id) : [];
        return new AgencyUserContextModel
        {
            AgencyId = personnel.AgencyId,
            AgencyPersonnelId = personnel.Id,
            AgencyIds = [personnel.AgencyId, .. childIds]
        };
    }

    public async Task<Common.Entities.Agency.Agency> GetAgencyMasterByLocation(CityModel city)
    {
        var country = await Context.Cities.AsNoTracking()
            .Where(c => c.Id == city.Id)
            .Select(c => c.Province.Country)
            .FirstOrDefaultAsync();
        if (country is null) return null;
        return await GetAgencyMasterByCountry(country.Id);
    }

    public async Task<Common.Entities.Agency.Agency> GetAgencyMasterByCountry(Guid countryId)
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

    public async Task<PaginatedList<AgencyModel>> GetAgencies(Guid agencyId, GetAgenciesFilter filter)
    {
        var query = GetAllAgencies(agencyId, filter);
        return await query.ToPaginatedList(filter);
    }

    public IQueryable<AgencyModel> GetAllAgencies(Guid agencyId, GetAgenciesFilter filter)
    {
        var agencies = Context.Agencies
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
            default:
                query = query.OrderBy(a => a.FullName);
                break;
        }

        return query;
    }

    public Task<UserAttendance> GetUserAttendance(Guid userId, DateTime date) =>
        Context.UserAttendances.FirstOrDefaultAsync(a => a.UserId == userId && a.Date == date.Date);

    public Task<UserAttendance> GetUserAttendanceForAgency(Guid id, Guid agencyId) =>
        UserAttendancesOf(agencyId).FirstOrDefaultAsync(a => a.Id == id);

    public Task<List<UserAttendance>> GetUserAttendancesForAgency(Guid agencyId, DateTime date) =>
        UserAttendancesOf(agencyId).Where(a => a.Date == date.Date).ToListAsync();

    public Task<List<UserAttendanceListModel>> GetUserAttendanceReport(Guid agencyId, GetUserAttendanceReportFilter filter)
    {
        var query = UserAttendancesOf(agencyId).Where(a => a.Date >= filter.From.Date && a.Date <= filter.To.Date);
        if (filter.UserId.HasValue) query = query.Where(a => a.UserId == filter.UserId.Value);
        return query
            .Select(a => new UserAttendanceListModel
            {
                Id = a.Id,
                UserId = a.UserId,
                Name = Context.AgencyPersonnel
                    .Where(p => p.AgencyId == agencyId && p.UserId == a.UserId)
                    .Select(p => p.Name)
                    .FirstOrDefault(),
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

    private IQueryable<UserAttendance> UserAttendancesOf(Guid agencyId) =>
        Context.UserAttendances.Where(a => Context.AgencyPersonnel.Any(p => p.AgencyId == agencyId && p.UserId == a.UserId));
}
