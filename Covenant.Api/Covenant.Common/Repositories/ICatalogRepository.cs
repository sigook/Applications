using Covenant.Common.Entities;
using Covenant.Common.Enums;
using Covenant.Common.Models;
using Covenant.Common.Models.Location;

namespace Covenant.Common.Repositories;

public interface ICatalogRepository
{
    Task Create<T>(T entity) where T: class;
    Task<List<BaseModel<Guid>>> GetWsibGroups();
    Task<List<CountryModel>> GetCountries();
    Task<List<ProvinceModel>> GetProvinces(Guid countryId);
    Task<List<ProvinceModel>> GetProvinces(string countryCode);
    Task<List<CityModel>> GetCities(Guid provinceId);
    Task<CityModel> GetCity(Guid cityId);
    Task<CityModel> GetCity(string name);
    Task<bool> CityExists(string name);
    Task<List<BaseModel<Guid>>> GetAvailability();
    Task<List<BaseModel<Guid>>> GetAvailabilityTime();
    Task<List<BaseModel<Guid>>> GetDay();
    Task<List<BaseModel<Guid>>> GetGender();
    Task<List<BaseModel<Guid>>> GetIdentificationType();
    Task<IdentificationTypeCode> GetIdentificationTypeCode(Guid id);
    Task<List<BaseModel<Guid>>> GetLanguage();
    Task<List<BaseModel<Guid>>> GetLift();
    Task<List<ReasonCancellationRequest>> GetReasonCancellationRequest();
    Task<List<BaseModel<Guid>>> GetIndustries();
    Task<List<BaseModel<Guid>>> GetSources();
    Task<List<BaseModel<Guid>>> GetSourcesForRequests();
    Task<bool> IsHoliday(DateTime date, string countryCode);
    Task<List<DateTime>> GetHolidaysInWeek(DateTime firstDateOfTheWeek, string countryCode);
    Task SaveChangesAsync();
    Task CreateHolidayIfNotExist(string countryCode, DateTime date);
    Task UpsertProvinceSettings(ProvinceSetting settings);
}