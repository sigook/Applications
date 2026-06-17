using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Location;
using System.Threading.Tasks;

namespace Covenant.Core.BL.Interfaces;

public interface ILocationService
{
    Task<List<CountryModel>> GetCountries();
    Task<List<ProvinceModel>> GetProvinces(Guid countryId);
    Task<List<ProvinceModel>> GetProvinces(string countryCode);
    Task<List<CityModel>> GetCities(Guid provinceId);
    Task<Result<Guid>> AddCity(CityModel model);
    Task<Result> UpsertProvinceSettings(Guid provinceId, ProvinceSettingsModel model);
    Task<LocationTaxModel> GetLocationTax(Guid locationId);
    Task<Result> UpsertLocationTax(Guid locationId, LocationTaxModel model);
}
