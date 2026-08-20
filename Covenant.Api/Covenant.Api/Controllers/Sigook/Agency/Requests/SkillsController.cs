using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Entities.Request;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Requests;

[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ApiController]
[ServiceFilter(typeof(AgencyIdFilter))]
public class SkillsController(IRequestRepository repository) : Controller
{
    public const string RouteName = "api/agency/requests/{requestId}/Skills";

    /// <summary>Adds a skill to the specified request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="model">Skill to add.</param>
    [HttpPost]
    [ProducesResponseType(typeof(SkillModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Post(Guid requestId, [FromBody] SkillModel model)
    {
        if (!ModelState.IsValid || string.IsNullOrEmpty(model?.Skill)) return BadRequest(ModelState);
        var result = RequestSkill.Create(requestId, model.Skill);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        await repository.Create<RequestSkill>([result.Value]);
        await repository.SaveChangesAsync();
        model.Id = result.Value.Id;
        return Ok(model);
    }

    /// <summary>Gets the skills of the specified request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SkillModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult> Get(Guid requestId) => Ok(await repository.GetSkills(requestId));

    /// <summary>Removes a skill from the specified request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="id">Identifier of the skill to remove.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Delete(Guid requestId, Guid id)
    {
        var entity = await repository.GetSkill(requestId, id);
        if (entity is null) return BadRequest();
        repository.Delete<RequestSkill>([entity]);
        await repository.SaveChangesAsync();
        return Ok();
    }
}
