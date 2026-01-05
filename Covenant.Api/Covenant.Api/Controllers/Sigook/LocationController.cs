using Covenant.Api.Authorization;
using Covenant.Common.Models.Location;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

    [HttpGet("country")]
    [ResponseCache(Duration = 3600)]
    public async Task<IActionResult> GetCountry()
    {
        var countries = await _locationService.GetCountries();
        return Ok(countries);
    }

    [HttpGet("province/{countryId:guid}")]
    [ResponseCache(Duration = 3600, VaryByQueryKeys = new[] { "countryId" })]
    public async Task<IActionResult> GetProvinces([FromRoute] Guid countryId)
    {
        var provinces = await _locationService.GetProvinces(countryId);
        return Ok(provinces);
    }

    [HttpGet("city/{provinceId}")]
    public async Task<IActionResult> GetCities([FromRoute] Guid provinceId)
    {
        var cities = await _locationService.GetCities(provinceId);
        return Ok(cities);
    }

    [Authorize(Policy = PolicyConfiguration.Agency)]
    [HttpPost("city")]
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
