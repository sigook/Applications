using Covenant.Api.Utils.Extensions;
using Covenant.Common.Entities;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Resources;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
            entity.UpdateTextSearch();
            await _workerRepository.UpdateProfile(entity);
            await _workerRepository.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        [Route("Availabilities")]
        public async Task<IActionResult> Availabilities(Guid profileId, [FromBody] List<BaseModel<Guid>> model) =>
            await CommonFunctionUpdate(model, profileId, entity => Task.FromResult(entity.PatchAvailabilities(model)));

        [HttpPost]
        [Route("AvailabilityDays")]
        public async Task<IActionResult> AvailabilityDays(Guid profileId, [FromBody] List<BaseModel<Guid>> model) =>
            await CommonFunctionUpdate(model, profileId, entity => Task.FromResult(entity.PatchAvailabilityDays(model)));

        [HttpPost]
        [Route("AvailabilityTimes")]
        public async Task<IActionResult> AvailabilityTimes(Guid profileId, [FromBody] List<BaseModel<Guid>> model) =>
            await CommonFunctionUpdate(model, profileId, entity => Task.FromResult(entity.PatchAvailabilityTimes(model)));

        [HttpPost]
        [Route("BasicInformation")]
        public async Task<IActionResult> BasicInformation(Guid profileId, [FromBody] WorkerProfileBasicInformationModel model) =>
            await CommonFunctionUpdate(model, profileId, entity => Task.FromResult(entity.PatchBasicInformation(model)));

        [HttpPost]
        [Route("Certificates")]
        public async Task<IActionResult> CreateCertificates(Guid profileId, [FromBody] List<CovenantFileModel> model) =>
            await CommonFunctionUpdate(model, profileId, entity => Task.FromResult(entity.PatchCertificates(model)));

        [HttpDelete]
        [Route("Certificates/{certificateId}")]
        public async Task<IActionResult> DeleteCertificate([FromRoute] Guid certificateId)
        {
            var certificate = await _workerRepository.GetCertificate(certificateId);
            _workerRepository.Delete(certificate);
            await _workerRepository.SaveChangesAsync();
            await documentService.DeleteFile(certificate.CertificateId);
            return Ok();
        }

        [HttpPost]
        [Route("ContactInformation")]
        public async Task<IActionResult> ContactInformation(Guid profileId, [FromBody] WorkerProfileContactInformation model) =>
            await CommonFunctionUpdate(model, profileId, entity => Task.FromResult(entity.PatchContactInformation(model)));

        [HttpPost]
        [Route("EmergencyInformation")]
        public async Task<IActionResult> EmergencyInformation(Guid profileId, [FromBody] EmergencyInformationModel model) =>
            await CommonFunctionUpdate(model, profileId, entity => Task.FromResult(entity.PatchEmergencyInformation(model)));

        [HttpPost]
        [Route("Languages")]
        public async Task<IActionResult> Languages(Guid profileId, [FromBody] List<BaseModel<Guid>> model) =>
            await CommonFunctionUpdate(model, profileId, entity => Task.FromResult(entity.PatchLanguages(model)));

        [HttpPost]
        [Route("Licenses")]
        public async Task<IActionResult> CreateLicenses(Guid profileId, [FromBody] List<WorkerProfileLicenseModel> model) =>
            await CommonFunctionUpdate(model, profileId, entity => Task.FromResult(entity.PatchLicenses(model)));

        [HttpDelete]
        [Route("Licenses/{licenseId}")]
        public async Task<IActionResult> DeleteLicenses([FromRoute] Guid licenseId)
        {
            var license = await _workerRepository.GetLicense(licenseId);
            _workerRepository.Delete(license);
            await _workerRepository.SaveChangesAsync();
            await documentService.DeleteFile(license.LicenseId);
            return Ok();
        }

        [HttpPost]
        [Route("LocationPreferences")]
        public async Task<IActionResult> LocationPreferences(Guid profileId, [FromBody] List<BaseModel<Guid>> model) =>
            await CommonFunctionUpdate(model, profileId, entity => Task.FromResult(entity.PatchLocationPreferences(model)));

        [HttpPost]
        [Route("OtherInformation")]
        public async Task<IActionResult> OtherInformation(Guid profileId, [FromBody] WorkerProfileOtherInformationModel model) =>
            await CommonFunctionUpdate(model, profileId, entity => Task.FromResult(entity.PatchOtherInformation(model)));

        [HttpPost]
        [Route("ProfileImage")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ProfileImage([FromRoute] Guid profileId)
        {
            var result = await _workerService.UpdateProfileImage(profileId);
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            return Ok();
        }

        [HttpPost]
        [Route("SinInformation")]
        public async Task<IActionResult> SinInformation(Guid profileId, [FromBody] SinInformationModel model) =>
            await CommonFunctionUpdate(model, profileId, async entity =>
            {
                entity.OnNewDocumentAdded += async (sender, args) => await _workerRepository.Create(args);
                return await Task.FromResult(entity.PatchSinInformation(model));
            });

        [HttpPost]
        [Route("Skills")]
        public async Task<IActionResult> Skills(Guid profileId, [FromBody] List<string> model) =>
            await CommonFunctionUpdate(model, profileId, entity => Task.FromResult(entity.PatchSkills(model)));

        [HttpPost]
        [Route("Documents")]
        public async Task<IActionResult> Documents(Guid profileId, [FromBody] DocumentsInformationModel model)
        {
            return await CommonFunctionUpdate(model, profileId, async entity =>
            {
                foreach (string number in new[] { model?.IdentificationNumber1, model?.IdentificationNumber2 })
                {
                    if (!string.IsNullOrEmpty(number) && await _workerRepository.InfoIsAlreadyTaken(x => x.Id != profileId && (x.IdentificationNumber1 == number || x.IdentificationNumber2 == number)))
                        return Result.Fail(string.Format(ApiResources.IdentificationNumberAlreadyTaken, number));
                }
                entity.OnNewDocumentAdded += async (sender, args) => await _workerRepository.Create(args);
                return entity.PatchDocuments(model);
            });
        }

        [HttpPost]
        [Route("Resume")]
        public async Task<IActionResult> Resume(Guid profileId, [FromBody] CovenantFileModel model) =>
            await CommonFunctionUpdate(model, profileId, entity =>
            {
                entity.OnNewDocumentAdded += async (sender, args) => await _workerRepository.Create(args);
                return Task.FromResult(entity.PatchResume(model));
            });

        [HttpPost]
        [Route("OtherDocument")]
        public async Task<IActionResult> CreateOtherDocument(Guid profileId, [FromBody] CovenantFileModel model)
        {
            var rDoc = CovenantFile.Create(model);
            if (!rDoc) return BadRequest();
            var entity = await _workerRepository.GetProfile(p => p.Id == profileId);
            if (entity is null) return BadRequest();
            var result = Covenant.Common.Entities.Worker.WorkerProfileOtherDocument.Create(entity.Id, rDoc.Value);
            if (!result) return BadRequest();
            await _workerRepository.Create(result.Value);
            await _workerRepository.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        [Route("OtherDocument/{otherDocumentId}")]
        public async Task<IActionResult> DeleteOtherDocument([FromRoute] Guid otherDocumentId)
        {
            var otherDocument = await _workerRepository.GetOtherDocument(otherDocumentId);
            _workerRepository.Delete(otherDocument);
            await _workerRepository.SaveChangesAsync();
            await documentService.DeleteFile(otherDocument.DocumentId);
            return Ok();
        }

        [HttpPost]
        [Route("PushNotificationsId")]
        public async Task<IActionResult> PushNotificationsId(Guid profileId, [FromBody] PushNotificationsIdModel model)
        {
            if (string.IsNullOrEmpty(model?.Id)) return BadRequest(ModelState.AddError("Id is required"));
            await System.IO.File.WriteAllTextAsync(Path.Combine(Path.GetTempPath(), $"{profileId:N}.txt"), $"{model.Id}-{model.Platform}");
            return Ok();
        }
    }
}