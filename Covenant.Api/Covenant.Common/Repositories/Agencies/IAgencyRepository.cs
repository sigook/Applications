using Covenant.Common.Entities.Agencies;
using Covenant.Common.Entities;
using Covenant.Common.Models.Agencies;
using Covenant.Common.Models.Locations;
using Covenant.Common.Models;

namespace Covenant.Common.Repositories.Agencies;

public interface IAgencyRepository : IBaseRepository
{
    Task<Agency> GetAgency(Guid id);
    Task<AgencyModel> GetAgencyDetail(Guid id);
    Task<bool> AgencyExists(Guid id);
    Task<List<AgencyPersonnelModel>> GetPersonnel(IEnumerable<Guid> userIds);
    Task<List<AgencyOptionModel>> GetAgencyOptions(IEnumerable<Guid> agencyIds);
    Task<List<LocationDetailModel>> GetLocations(Guid agencyId);
    Task<LocationDetailModel> GetLocationDetail(Guid agencyId, Guid id);
    Task<bool> PersonnelHasRequests(Guid userId);
    Task<bool> PersonnelHasCompanies(Guid userId);
    Task<Location> GetBillingLocation(Guid agencyId);
    Task<Agency> GetAgencyMasterByLocation(CityModel city);
    Task<Agency> GetAgencyMasterByCountry(Guid countryId);
    Task<PaginatedList<AgencyModel>> GetAgencies(IEnumerable<Guid> agencyIds, GetAgenciesFilter filter);
    IQueryable<AgencyModel> GetAllAgencies(IEnumerable<Guid> agencyIds, GetAgenciesFilter filter);
    Task<UserAttendance> GetUserAttendance(Guid userId, DateTime date);
    Task<UserAttendance> GetUserAttendanceOfUsers(Guid id, IEnumerable<Guid> userIds);
    Task<List<UserAttendance>> GetUserAttendancesOfUsers(IEnumerable<Guid> userIds, DateTime date);
    Task<List<UserAttendanceListModel>> GetUserAttendanceReport(IEnumerable<Guid> userIds, GetUserAttendanceReportFilter filter);
}