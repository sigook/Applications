using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Candidate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Candidates;

[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ApiController]
[ServiceFilter(typeof(AgencyIdFilter))]
public class SkillsController(ICandidateRepository candidateRepository) : Controller
{
    public const string RouteName = "api/agency/candidates/{candidateId}/Skills";

    /// <summary>Adds a skill to the specified candidate.</summary>
    /// <param name="candidateId">Identifier of the candidate.</param>
    /// <param name="model">Skill to add.</param>
    [HttpPost]
    [ProducesResponseType(typeof(SkillModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Post(Guid candidateId, [FromBody] SkillModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (string.IsNullOrEmpty(model?.Skill)) return BadRequest();
        var candidate = await candidateRepository.GetCandidate(c => c.Id == candidateId);
        if (candidate is null) return BadRequest();
        var result = candidate.AddSkill(model.Skill);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        await candidateRepository.Update(candidate);
        await candidateRepository.SaveChangesAsync();
        model.Id = result.Value.Id;
        return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
    }

    /// <summary>Gets the detail of a candidate skill by its identifier.</summary>
    /// <param name="candidateId">Identifier of the candidate.</param>
    /// <param name="id">Identifier of the skill.</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SkillModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetById(Guid candidateId, Guid id)
    {
        var skill = await candidateRepository.GetSkillDetail(candidateId, id);
        if (skill == null) return NotFound();
        return Ok(skill);
    }

    /// <summary>Deletes a skill from the specified candidate.</summary>
    /// <param name="candidateId">Identifier of the candidate.</param>
    /// <param name="id">Identifier of the skill to delete.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Delete(Guid candidateId, Guid id)
    {
        var candidate = await candidateRepository.GetCandidate(c => c.Id == candidateId);
        if (candidate is null) return BadRequest();
        var result = candidate.DeleteSkill(id);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        await candidateRepository.Update(candidate);
        await candidateRepository.SaveChangesAsync();
        return Ok();
    }
}
