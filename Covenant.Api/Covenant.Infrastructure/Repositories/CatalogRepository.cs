using Covenant.Common.Entities;
using Covenant.Common.Models;
using Covenant.Common.Models.Location;
using Covenant.Common.Repositories;
using Covenant.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Repositories
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly CovenantContext _context;

        public CatalogRepository(CovenantContext context) => _context = context;

        public async Task Create<T>(T entity) where T : class => await _context.Set<T>().AddAsync(entity);

        public Task<List<BaseModel<Guid>>> GetWsibGroups() =>
            _context.WsibGroups.OrderBy(c => c.Value).Select(c => new BaseModel<Guid>(c.Id, c.Value)).ToListAsync();

        public async Task<List<CountryModel>> GetCountries()
        {
            var query = _context.Countries.Select(c => new CountryModel
            {
                Id = c.Id,
                Value = c.Value,
                Code = c.Code
            }).OrderBy(c => c.Value);
            return await query.ToListAsync();
        }

        public async Task<List<ProvinceModel>> GetProvinces(Guid countryId) =>
            await _context.Provinces.AsNoTracking()
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
            await _context.Provinces.AsNoTracking()
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
            _context.Cities.Where(c => c.ProvinceId == provinceId)
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
            var result = await _context.Cities
                .Select(c => new CityModel
                {
                    Id = c.Id,
                    Value = c.Value,
                    Code = c.Code,
                    Province = new ProvinceModel
                    {
                        Code = c.Province.Code
                    }
                })
                .FirstOrDefaultAsync(c => c.Id == cityId);
            return result;
        }

        public async Task<CityModel> GetCity(string name)
        {
            var city = await _context.Cities
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
            return await _context.Cities
                .AnyAsync(c => c.Value.ToLower() == name.ToLower());
        }


        public Task<List<BaseModel<Guid>>> GetAvailability() =>
            _context.Availabilities.OrderBy(c => c.Value).Select(c => new BaseModel<Guid>(c.Id, c.Value)).ToListAsync();

        public Task<List<BaseModel<Guid>>> GetAvailabilityTime() =>
            _context.AvailabilityTimes.OrderBy(c => c.Value).Select(c => new BaseModel<Guid>(c.Id, c.Value)).ToListAsync();

        public Task<List<BaseModel<Guid>>> GetDay() =>
            _context.Days.Select(c => new BaseModel<Guid>(c.Id, c.Value)).ToListAsync();

        public Task<List<BaseModel<Guid>>> GetGender() =>
            _context.Genders.OrderBy(c => c.Value).Select(c => new BaseModel<Guid>(c.Id, c.Value)).ToListAsync();

        public Task<List<BaseModel<Guid>>> GetIdentificationType() =>
            _context.IdentificationTypes.OrderBy(c => c.Value).Select(c => new BaseModel<Guid>(c.Id, c.Value)).ToListAsync();

        public Task<List<BaseModel<Guid>>> GetLanguage() =>
            _context.Languages.OrderBy(c => c.Value).Select(c => new BaseModel<Guid>(c.Id, c.Value)).ToListAsync();

        public Task<List<BaseModel<Guid>>> GetLift() => _context.Lifts.OrderBy(c => c.Value).Select(c => new BaseModel<Guid>(c.Id, c.Value)).ToListAsync();

        public Task<List<BaseModel<Guid>>> GetIndustries() =>
            _context.Industries
            .Where(c => !c.IsDeleted)
            .OrderBy(d => d.Value)
            .Select(i => new BaseModel<Guid>(i.Id, i.Value))
            .ToListAsync();

        public Task<List<BaseModel<Guid>>> GetSources() =>
            _context.Sources
            .OrderBy(c => c.Value)
            .Select(c => new BaseModel<Guid>(c.Id, c.Value))
            .ToListAsync();

        public Task<List<BaseModel<Guid>>> GetSourcesForRequests() =>
            _context.Sources
            .Where(c => c.IsAvailableForRequests)
            .OrderBy(c => c.Value)
            .Select(c => new BaseModel<Guid>(c.Id, c.Value))
            .ToListAsync();

        public Task<List<ReasonCancellationRequest>> GetReasonCancellationRequest() =>
            _context.ReasonCancellationRequests.AsNoTracking().ToListAsync();

        public Task<bool> IsHoliday(DateTime date, string countryCode) => _context.Holidays.AnyAsync(c => c.Date.Date == date.Date && c.CountryCode == countryCode);

        public virtual Task<List<DateTime>> GetHolidaysInWeek(DateTime firstDateOfTheWeek, string countryCode)
        {
            var endOfWeek = firstDateOfTheWeek.Date.AddDays(7);
            return _context.Holidays.Where(holiday =>
                holiday.Date >= firstDateOfTheWeek.Date &&
                holiday.Date < endOfWeek &&
                holiday.CountryCode == countryCode
            ).Select(h => h.Date).ToListAsync();
        }

        public Task SaveChangesAsync() => _context.SaveChangesAsync();

        public async Task CreateHolidayIfNotExist(string countryCode, DateTime date)
        {
            var holiday = await _context.Holidays.FirstOrDefaultAsync(h => h.Date == date && h.CountryCode == countryCode);
            if (holiday == null)
            {
                holiday = new Holiday(date)
                {
                    CountryCode = countryCode
                };
                await _context.Holidays.AddAsync(holiday);
            }
        }

        public async Task UpsertProvinceSettings(ProvinceSetting settings)
        {
            var existing = await _context.ProvinceSettings.FindAsync(settings.ProvinceId);
            if (existing == null)
            {
                await _context.ProvinceSettings.AddAsync(settings);
            }
            else
            {
                existing.PaidHolidays = settings.PaidHolidays;
                existing.OvertimeStartsAfter = settings.OvertimeStartsAfter;
                _context.ProvinceSettings.Update(existing);
            }
        }
    }
}