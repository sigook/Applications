using Covenant.Common.Entities;
using Covenant.Common.Entities.Agency;
using Covenant.Common.Models;
using Covenant.Common.Models.Agency;
using Covenant.Common.Models.Location;

namespace Covenant.Common.Repositories.Agency;

public interface IAgencyRepository
{
    Task Create<T>(T entity) where T : class;
    void Update<T>(T entity) where T : class;
    Task<Entities.Agency.Agency> GetAgency(Guid id);
    Task<AgencyModel> GetAgencyDetail(Guid id);
    Task<bool> AgencyExists(Guid id);
    Task SaveChangesAsync();
    Task<IEnumerable<AgencyPersonnelModel>> GetAllPersonnel(Guid agencyId);
    Task<AgencyPersonnel> GetPersonnel(Guid id);
    Task<List<AgencyPersonnel>> GetPersonnelByUserId(Guid id);
    Task<AgencyPersonnelModel> GetPersonnel(Guid agencyId, Guid id);
    Task<Guid> GetAgencyIdForUser(Guid userId);
    Task<List<Guid>> GetAgencyIdsForUser(Guid userId);
    Task<List<LocationDetailModel>> GetLocations(Guid agencyId);
    Task<LocationDetailModel> GetLocationDetail(Guid agencyId, Guid id);
    Task DeletePersonnel(AgencyPersonnel entity);
    Task<IEnumerable<PersonnelAgencyModel>> GetPersonnelAgency(Guid userId);
    Task<Location> GetBillingLocation(Guid agencyId);
    Task<Entities.Agency.Agency> GetAgencyMasterByLocation(CityModel city);
    Task<Entities.Agency.Agency> GetAgencyMasterByCountry(Guid countryId);
    Task<PaginatedList<AgencyModel>> GetAgencies(Guid agencyId, GetAgenciesFilter filter);
    IEnumerable<AgencyModel> GetAllAgencies(Guid agencyId, GetAgenciesFilter filter);
}