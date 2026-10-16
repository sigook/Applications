using Covenant.Common.Entities;
using Covenant.Common.Enums;
using Covenant.Common.Models;
using Covenant.Common.Models.Location;
using Covenant.Common.Repositories;
using Covenant.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Repositories
{
    public class CatalogRepository(CovenantContext context) : BaseRepository<CovenantContext>(context), ICatalogRepository
    {
        public Task<List<BaseModel<Guid>>> GetWsibGroups() =>
            Context.WsibGroups.OrderBy(c => c.Value).Select(c => new BaseModel<Guid>(c.Id, c.Value)).ToListAsync();

        public async Task<List<CountryModel>> GetCountries()
        {
            var query = Context.Countries.Select(c => new CountryModel
            {
                Id = c.Id,
                Value = c.Value,
                Code = c.Code
            }).OrderBy(c => c.Value);
            return await query.ToListAsync();
        }

        public async Task<List<ProvinceModel>> GetProvinces(Guid countryId) =>
            await Context.Provinces.AsNoTracking()
                .Where(c => c.CountryId == countryId)
                .Select(s => new ProvinceModel
                {
                    Id = s.Id,
                    Value = s.Value,
                    Code = s.Code
                })
                .OrderBy(c => c.Value)
                .ToListAsync();
        public async Task<List<ProvinceModel>> GetProvinces(string countryCode) =>
            await Context.Provinces.AsNoTracking()
                .Where(c => c.Country.Code == countryCode)
                .Select(s => new ProvinceModel
                {
                    Id = s.Id,
                    Value = s.Value,
                    Code = s.Code
                })
                .OrderBy(c => c.Value)
                .ToListAsync();

        public Task<List<CityModel>> GetCities(Guid provinceId) =>
            Context.Cities.Where(c => c.ProvinceId == provinceId)
                .GroupBy(c => c.Value)
                .Select(c => new CityModel
                {
                    Id = c.FirstOrDefault().Id,
                    Value = c.Key,
                    Code = c.FirstOrDefault().Code,
                    Province = new ProvinceModel
                    {
                        Id = c.FirstOrDefault().Province.Id
                    }
                })
                .OrderBy(c => c.Value)
                .ToListAsync();

        public async Task<CityModel> GetCity(Guid cityId)
        {
            var result = await Context.Cities
                .Select(c => new CityModel
                {
                    Id = c.Id,
                    Value = c.Value,
                    Code = c.Code,
                    Province = new ProvinceModel
                    {
                        Id = c.Province.Id,
                        Value = c.Province.Value,
                        Code = c.Province.Code,
                        Country = new CountryModel
                        {
                            Id = c.Province.Country.Id,
                            Value = c.Province.Country.Value,
                            Code = c.Province.Country.Code
                        }
                    }
                })
                .FirstOrDefaultAsync(c => c.Id == cityId);
            return result;
        }

        public async Task<CityModel> GetCity(string name)
        {
            var city = await Context.Cities
                .Select(c => new CityModel
                {
                    Id = c.Id,
                    Value = c.Value,
                    Code = c.Code
                }).FirstOrDefaultAsync(c => c.Value.ToLower() == name.ToLower());
            return city;
        }

        public async Task<bool> CityExists(string name)
        {
            return await Context.Cities
                .AnyAsync(c => c.Value.ToLower() == name.ToLower());
        }


        public Task<List<BaseModel<Guid>>> GetAvailability() =>
            Context.Availabilities.OrderBy(c => c.Value).Select(c => new BaseModel<Guid>(c.Id, c.Value)).ToListAsync();

        public Task<List<BaseModel<Guid>>> GetAvailabilityTime() =>
            Context.AvailabilityTimes.OrderBy(c => c.Value).Select(c => new BaseModel<Guid>(c.Id, c.Value)).ToListAsync();

        public Task<List<BaseModel<Guid>>> GetDay() =>
            Context.Days.Select(c => new BaseModel<Guid>(c.Id, c.Value)).ToListAsync();

        public Task<List<BaseModel<Guid>>> GetGender() =>
            Context.Genders.OrderBy(c => c.Value).Select(c => new BaseModel<Guid>(c.Id, c.Value)).ToListAsync();

        public Task<List<IdentificationTypeModel>> GetIdentificationType() =>
            Context.IdentificationTypes.OrderBy(c => c.Value)
                .Select(c => new IdentificationTypeModel { Id = c.Id, Value = c.Value, Code = c.Code })
                .ToListAsync();

        public Task<IdentificationTypeCode> GetIdentificationTypeCode(Guid id) =>
            Context.IdentificationTypes.Where(c => c.Id == id).Select(c => c.Code).FirstOrDefaultAsync();

        public Task<List<BaseModel<Guid>>> GetLanguage() =>
            Context.Languages.OrderBy(c => c.Value).Select(c => new BaseModel<Guid>(c.Id, c.Value)).ToListAsync();

        public Task<List<BaseModel<Guid>>> GetLift() => Context.Lifts.OrderBy(c => c.Value).Select(c => new BaseModel<Guid>(c.Id, c.Value)).ToListAsync();

        public Task<List<BaseModel<Guid>>> GetIndustries() =>
            Context.Industries
            .Where(c => !c.IsDeleted)
            .OrderBy(d => d.Value)
            .Select(i => new BaseModel<Guid>(i.Id, i.Value))
            .ToListAsync();

        public Task<List<BaseModel<Guid>>> GetSources() =>
            Context.Sources
            .OrderBy(c => c.Value)
            .Select(c => new BaseModel<Guid>(c.Id, c.Value))
            .ToListAsync();

        public Task<List<BaseModel<Guid>>> GetSourcesForRequests() =>
            Context.Sources
            .Where(c => c.IsAvailableForRequests)
            .OrderBy(c => c.Value)
            .Select(c => new BaseModel<Guid>(c.Id, c.Value))
            .ToListAsync();

        public Task<List<ReasonCancellationRequest>> GetReasonCancellationRequest() =>
            Context.ReasonCancellationRequests.AsNoTracking().ToListAsync();

        public Task<bool> IsHoliday(DateTime date, string countryCode) => Context.Holidays.AnyAsync(c => c.Date.Date == date.Date && c.CountryCode == countryCode);

        public virtual Task<List<DateTime>> GetHolidaysInWeek(DateTime firstDateOfTheWeek, string countryCode)
        {
            var endOfWeek = firstDateOfTheWeek.Date.AddDays(7);
            return Context.Holidays.Where(holiday =>
                holiday.Date >= firstDateOfTheWeek.Date &&
                holiday.Date < endOfWeek &&
                holiday.CountryCode == countryCode
            ).Select(h => h.Date).ToListAsync();
        }

        public async Task CreateHolidayIfNotExist(string countryCode, DateTime date)
        {
            var holiday = await Context.Holidays.FirstOrDefaultAsync(h => h.Date == date && h.CountryCode == countryCode);
            if (holiday == null)
            {
                holiday = new Holiday(date)
                {
                    CountryCode = countryCode
                };
                await Context.Holidays.AddAsync(holiday);
            }
        }

        public async Task UpsertProvinceSettings(ProvinceSetting settings)
        {
            var existing = await Context.ProvinceSettings.FindAsync(settings.ProvinceId);
            if (existing == null)
            {
                await Context.ProvinceSettings.AddAsync(settings);
            }
            else
            {
                existing.PaidHolidays = settings.PaidHolidays;
                existing.OvertimeStartsAfter = settings.OvertimeStartsAfter;
                Context.ProvinceSettings.Update(existing);
            }
        }
    }
}