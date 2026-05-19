using Covenant.Api.Authorization;
using Covenant.Common.Models.Location;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook;

[Route(RouteName)]
[ApiController]
[AllowAnonymous]
public class LocationController : ControllerBase
{
    public const string RouteName = "api/Location";
    private readonly ILocationService _locationService;

    public LocationController(ILocationService locationService)
    {
        _locationService = locationService;
    }

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

    /// <summary>Gets the cities for the given province.</summary>
    /// <param name="provinceId">Province identifier.</param>
    [HttpGet("city/{provinceId}")]
    [ProducesResponseType(typeof(List<CityModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCities([FromRoute] Guid provinceId)
    {
        var cities = await _locationService.GetCities(provinceId);
        return Ok(cities);
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
