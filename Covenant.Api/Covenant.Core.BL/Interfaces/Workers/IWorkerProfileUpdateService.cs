using Covenant.Common.Functionals;
using Covenant.Common.Models.Workers;
using Covenant.Common.Models;

namespace Covenant.Core.BL.Interfaces.Workers;

public interface IWorkerProfileUpdateService
{
    Task<Result> UpdateAvailabilities(Guid profileId, List<BaseModel<Guid>> model);
    Task<Result> UpdateAvailabilityDays(Guid profileId, List<BaseModel<Guid>> model);
    Task<Result> UpdateAvailabilityTimes(Guid profileId, List<BaseModel<Guid>> model);
    Task<Result> UpdateBasicInformation(Guid profileId, WorkerProfileBasicInformationModel model);
    Task<Result> UpdateContactInformation(Guid profileId, WorkerProfileContactInformation model);
    Task<Result> UpdateEmergencyInformation(Guid profileId, EmergencyInformationModel model);
    Task<Result> UpdateLanguages(Guid profileId, List<BaseModel<Guid>> model);
    Task<Result> UpdateLocationPreferences(Guid profileId, List<BaseModel<Guid>> model);
    Task<Result> UpdateOtherInformation(Guid profileId, WorkerProfileOtherInformationModel model);
    Task<Result> UpdateSkills(Guid profileId, List<string> skills);
    Task<Result> AddJobExperience(Guid profileId, WorkerProfileJobExperienceModel model);
    Task<Result> UpdateJobExperience(Guid profileId, Guid id, WorkerProfileJobExperienceModel model);
    Task<Result> DeleteJobExperience(Guid profileId, Guid id);
    Task<Result> DeleteCertificate(Guid certificateId);
    Task<Result> DeleteLicense(Guid licenseId);
    Task<Result> DeleteOtherDocument(Guid otherDocumentId);
}
