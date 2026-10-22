using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Api.Utils;
using Covenant.Common.Entities.Agency;
using Covenant.Common.Models;
using Covenant.Common.Models.Agency;
using Covenant.Common.Repositories.Agencies;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces.Agencies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Agency.Sales;

[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ApiController]
[Produces("application/json")]
[ServiceFilter(typeof(AgencyIdFilter))]
public class AgenciesController(IAgencyService agencyService, IAgencyRepository agencyRepository) : ControllerBase
{
    public const string RouteName = "api/agency/sales/agencies";

    /// <summary>Gets a paginated list of agencies matching the given filter.</summary>
    /// <param name="filter">Filter and pagination criteria.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<AgencyModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAgencies([FromQuery] GetAgenciesFilter filter)
    {
        var data = await agencyRepository.GetAgencies(User.GetAgencyId(), filter);
        return Ok(data);
    }

    /// <summary>Gets the detail of a specific agency by id.</summary>
    /// <param name="agencyId">Agency identifier.</param>
    [HttpGet("{agencyId}")]
    [ProducesResponseType(typeof(AgencyModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAgency([FromRoute] Guid agencyId)
    {
        var agency = await agencyRepository.GetAgencyDetail(agencyId);
        return Ok(agency);
    }

    /// <summary>Creates a new agency.</summary>
    /// <param name="model">Agency data to create.</param>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAgency([FromBody] AgencyModel model)
    {
        var result = await agencyService.CreateAgency(model);
        if (result) return Ok();
        return BadRequest(ModelState.AddErrors(result.Errors));
    }
}
