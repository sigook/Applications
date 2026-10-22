using Covenant.Common.Entities.Workers;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Workers;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Workers;
using Covenant.Common.Resources;
using Covenant.Core.BL.Interfaces.Shared;
using Covenant.Core.BL.Interfaces.Workers;

namespace Covenant.Core.BL.Services.Workers;

public class WorkerProfileUpdateService(
    IWorkerRepository workerRepository,
    IDocumentService documentService,
    ISkillService skillService) : IWorkerProfileUpdateService
{
    public Task<Result> UpdateAvailabilities(Guid profileId, List<BaseModel<Guid>> model) =>
        Update(profileId, entity => Task.FromResult(entity.PatchAvailabilities(model)));

    public Task<Result> UpdateAvailabilityDays(Guid profileId, List<BaseModel<Guid>> model) =>
        Update(profileId, entity => Task.FromResult(entity.PatchAvailabilityDays(model)));

    public Task<Result> UpdateAvailabilityTimes(Guid profileId, List<BaseModel<Guid>> model) =>
        Update(profileId, entity => Task.FromResult(entity.PatchAvailabilityTimes(model)));

    public Task<Result> UpdateBasicInformation(Guid profileId, WorkerProfileBasicInformationModel model) =>
        Update(profileId, entity => Task.FromResult(entity.PatchBasicInformation(model)));

    public Task<Result> UpdateContactInformation(Guid profileId, WorkerProfileContactInformation model) =>
        Update(profileId, entity => Task.FromResult(entity.PatchContactInformation(model)));

    public Task<Result> UpdateEmergencyInformation(Guid profileId, EmergencyInformationModel model) =>
        Update(profileId, entity => Task.FromResult(entity.PatchEmergencyInformation(model)));

    public Task<Result> UpdateLanguages(Guid profileId, List<BaseModel<Guid>> model) =>
        Update(profileId, entity => Task.FromResult(entity.PatchLanguages(model)));

    public Task<Result> UpdateLocationPreferences(Guid profileId, List<BaseModel<Guid>> model) =>
        Update(profileId, entity => Task.FromResult(entity.PatchLocationPreferences(model)));

    public Task<Result> UpdateOtherInformation(Guid profileId, WorkerProfileOtherInformationModel model) =>
        Update(profileId, entity => Task.FromResult(entity.PatchOtherInformation(model)));

    public Task<Result> UpdateSkills(Guid profileId, List<string> skills) =>
        Update(profileId, async entity => entity.PatchSkills(await skillService.Canonicalize(skills)));

    public Task<Result> AddJobExperience(Guid profileId, WorkerProfileJobExperienceModel model) =>
        Update(profileId, entity => Task.FromResult<Result>(entity.AddJobExperience(model)));

    public Task<Result> UpdateJobExperience(Guid profileId, Guid id, WorkerProfileJobExperienceModel model) =>
        Update(profileId, entity => Task.FromResult(entity.UpdateJobExperience(id, model)));

    public Task<Result> DeleteJobExperience(Guid profileId, Guid id) =>
        Update(profileId, entity => Task.FromResult(entity.DeleteJobExperience(id)));

    public async Task<Result> DeleteCertificate(Guid certificateId)
    {
        var certificate = await workerRepository.GetCertificate(certificateId);
        if (certificate is null) return Result.Fail("Certificate not found");
        workerRepository.Delete(certificate);
        await workerRepository.SaveChangesAsync();
        await documentService.DeleteFile(certificate.CertificateId);
        return Result.Ok();
    }

    public async Task<Result> DeleteLicense(Guid licenseId)
    {
        var license = await workerRepository.GetLicense(licenseId);
        if (license is null) return Result.Fail("License not found");
        workerRepository.Delete(license);
        await workerRepository.SaveChangesAsync();
        await documentService.DeleteFile(license.LicenseId);
        return Result.Ok();
    }

    public async Task<Result> DeleteOtherDocument(Guid otherDocumentId)
    {
        var otherDocument = await workerRepository.GetOtherDocument(otherDocumentId);
        if (otherDocument is null) return Result.Fail("Document not found");
        workerRepository.Delete(otherDocument);
        await workerRepository.SaveChangesAsync();
        await documentService.DeleteFile(otherDocument.DocumentId);
        return Result.Ok();
    }

    private async Task<Result> Update(Guid profileId, Func<WorkerProfile, Task<Result>> update)
    {
        var entity = await workerRepository.GetProfile(p => p.Id == profileId);
        if (entity is null) return Result.Fail(ApiResources.WorkerNotFound);
        var result = await update(entity);
        if (!result) return result;
        workerRepository.Update(entity);
        await workerRepository.SaveChangesAsync();
        return Result.Ok();
    }
}
