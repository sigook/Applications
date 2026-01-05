using Covenant.Common.Functionals;
using Covenant.Common.Models.Location;

namespace Covenant.Core.BL.Interfaces;

public interface ILocationService
{
    Task<List<CountryModel>> GetCountries();
    Task<List<ProvinceModel>> GetProvinces(Guid countryId);
    Task<List<CityModel>> GetCities(Guid provinceId);
    Task<Result<Guid>> AddCity(CityModel model);
    Task<Result> UpsertProvinceSettings(Guid provinceId, ProvinceSettingsModel model);
}
