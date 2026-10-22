using Covenant.Api.Utils.Extensions;
using Covenant.Common.Entities;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Worker;
using Covenant.Core.BL.Interfaces.Workers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Worker.Profile;

[ApiController]
[Authorize]
[Route(RouteName)]
public class ProfileUpdateController(IWorkerService workerService, IWorkerProfileUpdateService workerProfileUpdateService) : ControllerBase
{
    public const string RouteName = "api/worker/profile/{profileId}";
    private readonly IWorkerService _workerService = workerService;

    private async Task<IActionResult> Update<T>(T model, Func<Task<Result>> update) where T : class
    {
        if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
        var result = await update();
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    private IActionResult ToActionResult(Result result) =>
        result ? Ok() : BadRequest(ModelState.AddErrors(result.Errors));

    /// <summary>
    /// Updates the availabilities of a worker profile.
    /// </summary>
    /// <param name="profileId">Identifier of the worker profile.</param>
    /// <param name="model">Collection of availability identifiers.</param>
    [HttpPost]
    [Route("Availabilities")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Availabilities(Guid profileId, [FromBody] List<BaseModel<Guid>> model) =>
        await Update(model, () => workerProfileUpdateService.UpdateAvailabilities(profileId, model));

    /// <summary>
    /// Updates the availability days of a worker profile.
    /// </summary>
    /// <param name="profileId">Identifier of the worker profile.</param>
    /// <param name="model">Collection of availability day identifiers.</param>
    [HttpPost]
    [Route("AvailabilityDays")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AvailabilityDays(Guid profileId, [FromBody] List<BaseModel<Guid>> model) =>
        await Update(model, () => workerProfileUpdateService.UpdateAvailabilityDays(profileId, model));

    /// <summary>
    /// Updates the availability times of a worker profile.
    /// </summary>
    /// <param name="profileId">Identifier of the worker profile.</param>
    /// <param name="model">Collection of availability time identifiers.</param>
    [HttpPost]
    [Route("AvailabilityTimes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AvailabilityTimes(Guid profileId, [FromBody] List<BaseModel<Guid>> model) =>
        await Update(model, () => workerProfileUpdateService.UpdateAvailabilityTimes(profileId, model));

    /// <summary>
    /// Updates the basic information of a worker profile.
    /// </summary>
    /// <param name="profileId">Identifier of the worker profile.</param>
    /// <param name="model">Basic information data.</param>
    [HttpPost]
    [Route("BasicInformation")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BasicInformation(Guid profileId, [FromBody] WorkerProfileBasicInformationModel model) =>
        await Update(model, () => workerProfileUpdateService.UpdateBasicInformation(profileId, model));

    /// <summary>
    /// Uploads certificate documents for a worker profile.
    /// </summary>
    /// <param name="profileId">Identifier of the worker profile.</param>
    [HttpPost]
    [Route("Certificates")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCertificates(Guid profileId)
    {
        var result = await _workerService.UpdateDocumentSection(profileId, WorkerDocumentType.Certificates);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>
    /// Deletes a certificate from a worker profile.
    /// </summary>
    /// <param name="certificateId">Identifier of the certificate.</param>
    [HttpDelete]
    [Route("Certificates/{certificateId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteCertificate([FromRoute] Guid certificateId) =>
        ToActionResult(await workerProfileUpdateService.DeleteCertificate(certificateId));

    /// <summary>
    /// Updates the contact information of a worker profile.
    /// </summary>
    /// <param name="profileId">Identifier of the worker profile.</param>
    /// <param name="model">Contact information data.</param>
    [HttpPost]
    [Route("ContactInformation")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ContactInformation(Guid profileId, [FromBody] WorkerProfileContactInformation model) =>
        await Update(model, () => workerProfileUpdateService.UpdateContactInformation(profileId, model));

    /// <summary>
    /// Updates the emergency information of a worker profile.
    /// </summary>
    /// <param name="profileId">Identifier of the worker profile.</param>
    /// <param name="model">Emergency information data.</param>
    [HttpPost]
    [Route("EmergencyInformation")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EmergencyInformation(Guid profileId, [FromBody] EmergencyInformationModel model) =>
        await Update(model, () => workerProfileUpdateService.UpdateEmergencyInformation(profileId, model));

    /// <summary>
    /// Updates the languages of a worker profile.
    /// </summary>
    /// <param name="profileId">Identifier of the worker profile.</param>
    /// <param name="model">Collection of language identifiers.</param>
    [HttpPost]
    [Route("Languages")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Languages(Guid profileId, [FromBody] List<BaseModel<Guid>> model) =>
        await Update(model, () => workerProfileUpdateService.UpdateLanguages(profileId, model));

    /// <summary>
    /// Uploads license documents for a worker profile.
    /// </summary>
    /// <param name="profileId">Identifier of the worker profile.</param>
    [HttpPost]
    [Route("Licenses")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateLicenses(Guid profileId)
    {
        var result = await _workerService.UpdateDocumentSection(profileId, WorkerDocumentType.Licenses);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>
    /// Deletes a license from a worker profile.
    /// </summary>
    /// <param name="licenseId">Identifier of the license.</param>
    [HttpDelete]
    [Route("Licenses/{licenseId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteLicenses([FromRoute] Guid licenseId) =>
        ToActionResult(await workerProfileUpdateService.DeleteLicense(licenseId));

    /// <summary>
    /// Updates the location preferences of a worker profile.
    /// </summary>
    /// <param name="profileId">Identifier of the worker profile.</param>
    /// <param name="model">Collection of location preference identifiers.</param>
    [HttpPost]
    [Route("LocationPreferences")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LocationPreferences(Guid profileId, [FromBody] List<BaseModel<Guid>> model) =>
        await Update(model, () => workerProfileUpdateService.UpdateLocationPreferences(profileId, model));

    /// <summary>
    /// Updates other information of a worker profile.
    /// </summary>
    /// <param name="profileId">Identifier of the worker profile.</param>
    /// <param name="model">Other information data.</param>
    [HttpPost]
    [Route("OtherInformation")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> OtherInformation(Guid profileId, [FromBody] WorkerProfileOtherInformationModel model) =>
        await Update(model, () => workerProfileUpdateService.UpdateOtherInformation(profileId, model));

    /// <summary>
    /// Uploads or updates the profile image of a worker profile.
    /// </summary>
    /// <param name="profileId">Identifier of the worker profile.</param>
    [HttpPost]
    [Route("ProfileImage")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ProfileImage([FromRoute] Guid profileId)
    {
        var result = await _workerService.UpdateProfileImage(profileId);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>
    /// Uploads social insurance documents for a worker profile.
    /// </summary>
    /// <param name="profileId">Identifier of the worker profile.</param>
    [HttpPost]
    [Route("SinInformation")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SinInformation(Guid profileId)
    {
        var result = await _workerService.UpdateDocumentSection(profileId, WorkerDocumentType.SocialInsurance);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>
    /// Updates the skills of a worker profile.
    /// </summary>
    /// <param name="profileId">Identifier of the worker profile.</param>
    /// <param name="model">Collection of skill names.</param>
    [HttpPost]
    [Route("Skills")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Skills(Guid profileId, [FromBody] List<string> model) =>
        await Update(model, () => workerProfileUpdateService.UpdateSkills(profileId, model));

    /// <summary>
    /// Uploads identification documents for a worker profile.
    /// </summary>
    /// <param name="profileId">Identifier of the worker profile.</param>
    [HttpPost]
    [Route("Documents")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Documents(Guid profileId)
    {
        var result = await _workerService.UpdateDocumentSection(profileId, WorkerDocumentType.Identification);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>
    /// Uploads the resume document for a worker profile.
    /// </summary>
    /// <param name="profileId">Identifier of the worker profile.</param>
    [HttpPost]
    [Route("Resume")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Resume(Guid profileId)
    {
        var result = await _workerService.UpdateDocumentSection(profileId, WorkerDocumentType.Resume);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>
    /// Uploads other documents for a worker profile.
    /// </summary>
    /// <param name="profileId">Identifier of the worker profile.</param>
    [HttpPost]
    [Route("OtherDocument")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOtherDocument(Guid profileId)
    {
        var result = await _workerService.UpdateDocumentSection(profileId, WorkerDocumentType.OtherDocument);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>
    /// Deletes an other document from a worker profile.
    /// </summary>
    /// <param name="otherDocumentId">Identifier of the document.</param>
    [HttpDelete]
    [Route("OtherDocument/{otherDocumentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteOtherDocument([FromRoute] Guid otherDocumentId) =>
        ToActionResult(await workerProfileUpdateService.DeleteOtherDocument(otherDocumentId));
}
