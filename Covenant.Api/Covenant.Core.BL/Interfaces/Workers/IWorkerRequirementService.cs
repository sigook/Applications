using Covenant.Common.Enums;
using Covenant.Common.Models.Worker;

namespace Covenant.Core.BL.Interfaces.Workers;

public interface IWorkerRequirementService
{
    IReadOnlyList<WorkerRequirement> GetMissing(IWorkerRequirementsInformation worker);
    IReadOnlyList<WorkerRequirement> GetMissingDocuments(IWorkerRequirementsInformation worker);
}
