using Covenant.Common.Entities;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Location;
using Covenant.Common.Repositories;
using Covenant.Core.BL.Interfaces;
using Microsoft.Extensions.Logging;

namespace Covenant.Core.BL.Services;

public class LocationService : ILocationService
{
    private readonly ICatalogRepository _catalogRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<LocationService> _logger;

    public LocationService(
        ICatalogRepository catalogRepository,
        ILocationRepository locationRepository,
        ILogger<LocationService> logger)
    {
        _catalogRepository = catalogRepository;
        _locationRepository = locationRepository;
        _logger = logger;
    }

    public async Task<List<CountryModel>> GetCountries()
    {
        return await _catalogRepository.GetCountries();
    }

    public async Task<List<ProvinceModel>> GetProvinces(Guid countryId)
    {
        return await _catalogRepository.GetProvinces(countryId);
    }

    public async Task<List<CityModel>> GetCities(Guid provinceId)
    {
        return await _catalogRepository.GetCities(provinceId);
    }

    public async Task<Result<Guid>> AddCity(CityModel model)
    {
        try
        {
            if (model == null)
            {
                return Result.Fail<Guid>("City model cannot be null");
            }

            if (string.IsNullOrWhiteSpace(model.Value))
            {
                return Result.Fail<Guid>("City name is required");
            }

            if (model.Province?.Id == Guid.Empty)
            {
                return Result.Fail<Guid>("Province is required");
            }

            var city = new City
            {
                Value = model.Value.ToUpper(),
                ProvinceId = model.Province.Id
            };

            await _catalogRepository.Create(city);
            await _catalogRepository.SaveChangesAsync();

            return Result.Ok(city.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating city: {CityName}", model?.Value);
            return Result.Fail<Guid>("An error occurred while creating the city");
        }
    }

    public async Task<Result> UpsertProvinceSettings(Guid provinceId, ProvinceSettingsModel model)
    {
        try
        {
            if (provinceId == Guid.Empty)
            {
                return Result.Fail("Province ID is required");
            }

            if (model == null)
            {
                return Result.Fail("Province settings model cannot be null");
            }

            var settings = new ProvinceSetting
            {
                ProvinceId = provinceId,
                PaidHolidays = model.PaidHolidays,
                OvertimeStartsAfter = model.OvertimeStartsAfter.HasValue
                    ? TimeSpan.FromHours(model.OvertimeStartsAfter.Value)
                    : null
            };

            await _catalogRepository.UpsertProvinceSettings(settings);
            await _catalogRepository.SaveChangesAsync();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error upserting province settings for province: {ProvinceId}", provinceId);
            return Result.Fail("An error occurred while saving province settings");
        }
    }

    public async Task<LocationTaxModel> GetLocationTax(Guid locationId)
    {
        var tax = await _locationRepository.GetLocationTax(locationId);
        if (tax is null)
        {
            return null;
        }

        tax.Tax1 *= 100m;
        return tax;
    }

    public async Task<Result> UpsertLocationTax(Guid locationId, LocationTaxModel model)
    {
        await _locationRepository.UpsertLocationTax(locationId, model.Tax1 / 100m);
        await _locationRepository.SaveChangesAsync();

        return Result.Ok();
    }
}
