using Covenant.Api.Authorization;
using Covenant.Common.Models.Location;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook;

[Route(RouteName)]
[ApiController]
[AllowAnonymous]
public class LocationController(ILocationService locationService) : ControllerBase
{
    public const string RouteName = "api/Location";
    private readonly ILocationService _locationService = locationService;

    /// <summary>Gets the list of available countries.</summary>
    [HttpGet("country")]
    [ResponseCache(Duration = 3600)]
    [ProducesResponseType(typeof(List<CountryModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCountry()
    {
        var countries = await _locationService.GetCountries();
        return Ok(countries);
    }

    /// <summary>Gets the provinces for the given country.</summary>
    /// <param name="countryId">Country identifier.</param>
    [HttpGet("province/{countryId:guid}")]
    [ResponseCache(Duration = 3600, VaryByQueryKeys = new[] { "countryId" })]
    [ProducesResponseType(typeof(List<ProvinceModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProvinces([FromRoute] Guid countryId)
    {
        var provinces = await _locationService.GetProvinces(countryId);
        return Ok(provinces);
    }

    /// <summary>Gets the provinces for the given country by Country Code.</summary>
    /// <param name="countryCode">Country identifier.</param>
    [HttpGet("province/{countryCode}")]
    [ResponseCache(Duration = 3600, VaryByQueryKeys = new[] { "countryCode" })]
    [ProducesResponseType(typeof(List<ProvinceModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GetProvinces([FromRoute] string countryCode)
    {
        var code = countryCode.ToUpperInvariant();
        Console.WriteLine(code);

        if (code is not "CA" and not "USA")
        {
            Console.WriteLine(code);
            return UnprocessableEntity(new
            {
                Code = countryCode,
                Error = "Muste be either CA or USA"
            });
        }

        var provinces = await _locationService.GetProvinces(code);
        return Ok(provinces);
    }

    /// <summary>Gets the cities for the given province.</summary>
    /// <param name="provinceId">Province identifier.</param>
    [HttpGet("city/{provinceId}")]
    [ProducesResponseType(typeof(List<CityModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCities([FromRoute] Guid provinceId)
    {
        var cities = await _locationService.GetCities(provinceId);
        return Ok(cities);
    }

    /// <summary>Creates or updates the settings for the given province.</summary>
    /// <param name="provinceId">Province identifier.</param>
    /// <param name="model">Province settings.</param>
    [Authorize(Policy = PolicyConfiguration.Admin)]
    [HttpPut("province/{provinceId:guid}/settings")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpsertProvinceSettings([FromRoute] Guid provinceId, [FromBody] ProvinceSettingsModel model)
    {
        var result = await _locationService.UpsertProvinceSettings(provinceId, model);
        if (!result)
        {
            return BadRequest(result.Errors);
        }
        return Ok();
    }

    /// <summary>Adds a new city.</summary>
    /// <param name="model">City data to create.</param>
    [Authorize(Policy = PolicyConfiguration.Agency)]
    [HttpPost("city")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddCity([FromBody] CityModel model)
    {
        var result = await _locationService.AddCity(model);
        if (!result)
        {
            return BadRequest(result.Errors);
        }
        return Ok(new { Id = result.Value });
    }
}
