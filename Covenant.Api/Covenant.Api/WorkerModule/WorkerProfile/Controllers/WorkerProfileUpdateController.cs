using Covenant.Api.Utils.Extensions;
using Covenant.Common.Entities;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Resources;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.WorkerModule.WorkerProfile.Controllers
{
    [ApiController]
    [Authorize]
    [Route(RouteName)]
    public class WorkerProfileUpdateController : ControllerBase
    {
        public const string RouteName = "api/WorkerProfile/{profileId}";
        private readonly IWorkerRepository _workerRepository;
        private readonly IDocumentService documentService;
        private readonly IWorkerService _workerService;

        public WorkerProfileUpdateController(IWorkerRepository workerRepository, IDocumentService documentService, IWorkerService workerService)
        {
            _workerRepository = workerRepository;
            this.documentService = documentService;
            _workerService = workerService;
        }

        private async Task<IActionResult> CommonFunctionUpdate<T>(T model, Guid profileId, Func<Covenant.Common.Entities.Worker.WorkerProfile, Task<Result>> update) where T : class
        {
            if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
            var entity = await _workerRepository.GetProfile(p => p.Id == profileId);
            if (entity is null) return BadRequest();
            Result result = await update(entity);
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            await _workerRepository.UpdateProfile(entity);
            await _workerRepository.SaveChangesAsync();
            return Ok();
        }

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
            await CommonFunctionUpdate(model, profileId, entity => Task.FromResult(entity.PatchAvailabilities(model)));

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
            await CommonFunctionUpdate(model, profileId, entity => Task.FromResult(entity.PatchAvailabilityDays(model)));

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
            await CommonFunctionUpdate(model, profileId, entity => Task.FromResult(entity.PatchAvailabilityTimes(model)));

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
            await CommonFunctionUpdate(model, profileId, entity => Task.FromResult(entity.PatchBasicInformation(model)));

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
        public async Task<IActionResult> DeleteCertificate([FromRoute] Guid certificateId)
        {
            var certificate = await _workerRepository.GetCertificate(certificateId);
            _workerRepository.Delete(certificate);
            await _workerRepository.SaveChangesAsync();
            await documentService.DeleteFile(certificate.CertificateId);
            return Ok();
        }

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
            await CommonFunctionUpdate(model, profileId, entity => Task.FromResult(entity.PatchContactInformation(model)));

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
            await CommonFunctionUpdate(model, profileId, entity => Task.FromResult(entity.PatchEmergencyInformation(model)));

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
            await CommonFunctionUpdate(model, profileId, entity => Task.FromResult(entity.PatchLanguages(model)));

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
        public async Task<IActionResult> DeleteLicenses([FromRoute] Guid licenseId)
        {
            var license = await _workerRepository.GetLicense(licenseId);
            _workerRepository.Delete(license);
            await _workerRepository.SaveChangesAsync();
            await documentService.DeleteFile(license.LicenseId);
            return Ok();
        }

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
            await CommonFunctionUpdate(model, profileId, entity => Task.FromResult(entity.PatchLocationPreferences(model)));

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
            await CommonFunctionUpdate(model, profileId, entity => Task.FromResult(entity.PatchOtherInformation(model)));

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
            await CommonFunctionUpdate(model, profileId, entity => Task.FromResult(entity.PatchSkills(model)));

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
        public async Task<IActionResult> DeleteOtherDocument([FromRoute] Guid otherDocumentId)
        {
            var otherDocument = await _workerRepository.GetOtherDocument(otherDocumentId);
            _workerRepository.Delete(otherDocument);
            await _workerRepository.SaveChangesAsync();
            await documentService.DeleteFile(otherDocument.DocumentId);
            return Ok();
        }

        /// <summary>
        /// Registers the push notifications device identifier for a worker profile.
        /// </summary>
        /// <param name="profileId">Identifier of the worker profile.</param>
        /// <param name="model">Push notifications device data.</param>
        [HttpPost]
        [Route("PushNotificationsId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PushNotificationsId(Guid profileId, [FromBody] PushNotificationsIdModel model)
        {
            if (string.IsNullOrEmpty(model?.Id)) return BadRequest(ModelState.AddError("Id is required"));
            await System.IO.File.WriteAllTextAsync(Path.Combine(Path.GetTempPath(), $"{profileId:N}.txt"), $"{model.Id}-{model.Platform}");
            return Ok();
        }
    }
}