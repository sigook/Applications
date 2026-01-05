using Covenant.Common.Entities.Worker;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Adapters;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models.Security;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Resources;
using Microsoft.AspNetCore.Http;
using UserType = Covenant.Common.Enums.UserType;

namespace Covenant.Core.BL.Adapters;

public class WorkerAdapter : IWorkerAdapter
{
    private readonly IAgencyRepository agencyRepository;
    private readonly IIdentityServerService identityServerService;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IFilesContainer filesContainer;

    public WorkerAdapter(
        IAgencyRepository agencyRepository,
        IIdentityServerService identityServerService,
        IHttpContextAccessor httpContextAccessor,
        IFilesContainer filesContainer)
    {
        this.agencyRepository = agencyRepository;
        this.identityServerService = identityServerService;
        this.httpContextAccessor = httpContextAccessor;
        this.filesContainer = filesContainer;
    }

    public async Task<Result<WorkerProfile>> MapToWorkerProfile(WorkerProfileCreateModel model)
    {
        var request = httpContextAccessor.HttpContext.Request;
        var entity = new WorkerProfile();
        var validationResult = entity.PatchBasicInformation(model);
        if (!validationResult) return Result.Fail<WorkerProfile>(validationResult.Errors);
        validationResult = entity.PatchContactInformation(model);
        if (!validationResult) return Result.Fail<WorkerProfile>(validationResult.Errors);
        validationResult = entity.PatchAvailabilities(model.Availabilities.ToList());
        if (!validationResult) return Result.Fail<WorkerProfile>(validationResult.Errors);
        validationResult = entity.PatchAvailabilityTimes(model.AvailabilityTimes.ToList());
        if (!validationResult) return Result.Fail<WorkerProfile>(validationResult.Errors);
        validationResult = entity.PatchAvailabilityDays(model.AvailabilityDays.ToList());
        if (!validationResult) return Result.Fail<WorkerProfile>(validationResult.Errors);
        validationResult = entity.PatchLocationPreferences(model.LocationPreferences.ToList());
        if (!validationResult) return Result.Fail<WorkerProfile>(validationResult.Errors);
        validationResult = entity.PatchLanguages(model.Languages.ToList());
        if (!validationResult) return Result.Fail<WorkerProfile>(validationResult.Errors);
        validationResult = entity.PatchSkills(model.Skills.Select(m => m.Skill).ToList());
        if (!validationResult) return Result.Fail<WorkerProfile>(validationResult.Errors);
        validationResult = entity.PatchOtherInformation(model);
        if (!validationResult) return Result.Fail<WorkerProfile>(validationResult.Errors);
        validationResult = entity.PatchDocuments(model);
        if (!validationResult) return Result.Fail<WorkerProfile>(validationResult.Errors);
        validationResult = entity.PatchLicenses(model.Licenses.ToList());
        if (!validationResult) return Result.Fail<WorkerProfile>(validationResult.Errors);
        validationResult = entity.PatchCertificates(model.Certificates.ToList());
        if (!validationResult) return Result.Fail<WorkerProfile>(validationResult.Errors);
        validationResult = entity.AddOtherDocuments(model.OtherDocuments.ToList());
        if (!validationResult) return Result.Fail<WorkerProfile>(validationResult.Errors);
        validationResult = entity.PatchProfileImage(model.ProfileImage);
        if (!validationResult) return Result.Fail<WorkerProfile>(validationResult.Errors);
        var agency = default(Common.Entities.Agency.Agency);
        var agencyId = identityServerService.GetAgencyId();
        if (agencyId == Guid.Empty)
        {
            agency = await agencyRepository.GetAgencyMasterByLocation(model.Location.City);
            if (agency is null) return Result.Fail<WorkerProfile>(ApiResources.AgencyNotFound);
        }
        else
        {
            agency = await agencyRepository.GetAgency(agencyId);
        }
        entity.AgencyId = agencyId;
        entity.Agency = agency;
        entity.ApprovedToWork = false;
        entity.UpdateTextSearch();
        var user = await identityServerService.CreateUser(new CreateUserModel
        {
            Email = model.Email,
            Password = model.Password,
            ConfirmPassword = model.ConfirmPassword,
            UserType = UserType.Worker
        });
        if (!user) return Result.Fail<WorkerProfile>(user.Errors);
        entity.Worker = user.Value;
        var profileImageFile = request.Form.Files[entity.ProfileImage.FileName];
        if (profileImageFile != null) await filesContainer.UploadAsync(profileImageFile.OpenReadStream(), profileImageFile.FileName);
        var identificationType1File = request.Form.Files[entity.IdentificationType1File.FileName];
        if (identificationType1File != null) await filesContainer.UploadAsync(identificationType1File.OpenReadStream(), identificationType1File.FileName);
        var identificationType2File = request.Form.Files[entity.IdentificationType2File?.FileName];
        if (identificationType2File != null) await filesContainer.UploadAsync(identificationType2File.OpenReadStream(), identificationType2File.FileName);
        var resume = request.Form.Files[entity.Resume?.FileName];
        if (resume != null) await filesContainer.UploadAsync(resume.OpenReadStream(), entity.Resume.FileName);
        foreach (var license in entity.Licenses)
        {
            var licenseFile = request.Form.Files[license.License.FileName];
            if (licenseFile != null) await filesContainer.UploadAsync(licenseFile.OpenReadStream(), license.License.FileName);
        }
        foreach (var certificate in entity.Certificates)
        {
            var certificateFile = request.Form.Files[certificate.Certificate.FileName];
            if (certificateFile != null) await filesContainer.UploadAsync(certificateFile.OpenReadStream(), certificate.Certificate.FileName);
        }
        foreach (var otherDocument in entity.OtherDocuments)
        {
            var otherDocumentFile = request.Form.Files[otherDocument.Document.FileName];
            if (otherDocumentFile != null) await filesContainer.UploadAsync(otherDocumentFile.OpenReadStream(), otherDocument.Document.FileName);
        }
        return Result.Ok(entity);
    }
}
