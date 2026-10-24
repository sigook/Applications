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
    Task<IEnumerable<AgencyPersonnelModel>> GetAllPersonnel(Guid agencyId);
    Task<AgencyPersonnel> GetPersonnel(Guid id);
    Task<List<AgencyPersonnel>> GetPersonnels(IEnumerable<Guid> ids);
    Task<List<AgencyPersonnel>> GetPersonnelByUserId(Guid id);
    Task<AgencyPersonnelModel> GetPersonnel(Guid agencyId, Guid id);
    Task<AgencyUserContextModel> GetAgencyContextForUser(Guid userId);
    Task<List<LocationDetailModel>> GetLocations(Guid agencyId);
    Task<LocationDetailModel> GetLocationDetail(Guid agencyId, Guid id);
    Task<bool> PersonnelHasRequests(Guid personnelId);
    Task<bool> PersonnelHasCompanies(Guid personnelId);
    Task<IEnumerable<PersonnelAgencyModel>> GetPersonnelAgency(Guid userId);
    Task<Location> GetBillingLocation(Guid agencyId);
    Task<Agency> GetAgencyMasterByLocation(CityModel city);
    Task<Agency> GetAgencyMasterByCountry(Guid countryId);
    Task<PaginatedList<AgencyModel>> GetAgencies(Guid agencyId, GetAgenciesFilter filter);
    IQueryable<AgencyModel> GetAllAgencies(Guid agencyId, GetAgenciesFilter filter);
    Task<UserAttendance> GetUserAttendance(Guid userId, DateTime date);
    Task<UserAttendance> GetUserAttendanceForAgency(Guid id, Guid agencyId);
    Task<List<UserAttendance>> GetUserAttendancesForAgency(Guid agencyId, DateTime date);
    Task<List<UserAttendanceListModel>> GetUserAttendanceReport(Guid agencyId, GetUserAttendanceReportFilter filter);
}