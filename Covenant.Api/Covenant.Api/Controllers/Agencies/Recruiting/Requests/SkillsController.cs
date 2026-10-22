using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models;
using Covenant.Core.BL.Interfaces.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Agencies.Recruiting.Requests;

[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ApiController]
[ServiceFilter(typeof(AgencyIdFilter))]
public class SkillsController(IRequestDetailsService requestDetailsService) : Controller
{
    public const string RouteName = "api/agency/recruiting/requests/{requestId}/skills";

    /// <summary>Adds a skill to the specified request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="model">Skill to add.</param>
    [HttpPost]
    [ProducesResponseType(typeof(SkillModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Post(Guid requestId, [FromBody] SkillModel model)
    {
        if (!ModelState.IsValid || string.IsNullOrEmpty(model?.Skill)) return BadRequest(ModelState);
        var result = await requestDetailsService.AddSkill(requestId, model.Skill);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok(result.Value);
    }

    /// <summary>Removes a skill from the specified request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="id">Identifier of the skill to remove.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Delete(Guid requestId, Guid id)
    {
        var result = await requestDetailsService.DeleteSkill(requestId, id);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }
}
