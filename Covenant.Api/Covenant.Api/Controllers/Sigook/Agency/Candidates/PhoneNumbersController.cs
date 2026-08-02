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
public class PhoneNumbersController(ICandidateRepository candidateRepository) : Controller
{
    public const string RouteName = "api/agency/candidates/{candidateId}/PhoneNumbers";

    /// <summary>Adds a phone number to the specified candidate.</summary>
    /// <param name="candidateId">Identifier of the candidate.</param>
    /// <param name="model">Phone number to add.</param>
    [HttpPost]
    [ProducesResponseType(typeof(PhoneNumberModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Post(Guid candidateId, [FromBody] PhoneNumberModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var entity = await candidateRepository.GetCandidate(c => c.Id == candidateId);
        if (entity is null) return BadRequest();
        var result = entity.AddPhone(model.PhoneNumber);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        await candidateRepository.Update(entity);
        await candidateRepository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, new PhoneNumberModel { Id = result.Value.Id });
    }

    /// <summary>Gets the detail of a candidate phone number by its identifier.</summary>
    /// <param name="candidateId">Identifier of the candidate.</param>
    /// <param name="id">Identifier of the phone number.</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PhoneNumberModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetById(Guid candidateId, Guid id)
    {
        var phoneNumber = await candidateRepository.GetPhoneNumberDetail(candidateId, id);
        if (phoneNumber == null) return NotFound();
        return Ok(phoneNumber);
    }

    /// <summary>Deletes a phone number from the specified candidate.</summary>
    /// <param name="candidateId">Identifier of the candidate.</param>
    /// <param name="id">Identifier of the phone number to delete.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Delete(Guid candidateId, Guid id)
    {
        var entity = await candidateRepository.GetCandidate(c => c.Id == candidateId);
        if (entity is null) return BadRequest();
        var result = entity.DeletePhone(id);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        await candidateRepository.Update(entity);
        await candidateRepository.SaveChangesAsync();
        return Ok();
    }
}
