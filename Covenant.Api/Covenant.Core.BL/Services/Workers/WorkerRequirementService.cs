using Covenant.Common.Enums;
using Covenant.Common.Models.Workers;
using Covenant.Core.BL.Interfaces.Workers;

namespace Covenant.Core.BL.Services.Workers;

public class WorkerRequirementService(TimeProvider timeProvider) : IWorkerRequirementService
{
    public IReadOnlyList<WorkerRequirement> GetMissing(IWorkerRequirementsInformation worker)
    {
        if (worker.IsSubcontractor || worker.IsContractor) return [];
        return GetMissingDocuments(worker);
    }

    public IReadOnlyList<WorkerRequirement> GetMissingDocuments(IWorkerRequirementsInformation worker)
    {
        var today = timeProvider.GetLocalNow().Date;
        var missing = new List<WorkerRequirement>();
        if (!worker.HasProfileImage) missing.Add(WorkerRequirement.ProfileImage);
        if (!worker.IsUsa && !worker.HasTaxCategory) missing.Add(WorkerRequirement.TaxCategory);
        if (string.IsNullOrWhiteSpace(worker.SocialInsurance)) missing.Add(WorkerRequirement.SocialInsuranceNumber);
        else if (worker.SocialInsuranceExpire && worker.DueDate.GetValueOrDefault().Date < today) missing.Add(WorkerRequirement.SocialInsuranceExpired);
        if (!worker.HasSocialInsuranceFile) missing.Add(WorkerRequirement.SocialInsuranceFile);
        if (!HasIdentification(worker)) missing.Add(WorkerRequirement.Identification);
        return missing;
    }

    private static bool HasIdentification(IWorkerRequirementsInformation worker) =>
        (!string.IsNullOrWhiteSpace(worker.IdentificationNumber1) && worker.HasIdentification1File) ||
        (!string.IsNullOrWhiteSpace(worker.IdentificationNumber2) && worker.HasIdentification2File);
}
