using Covenant.Common.Entities.Worker;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Worker;

namespace Covenant.Common.Interfaces.Adapters;

public interface IWorkerAdapter
{
    Task<Result<WorkerProfile>> MapToWorkerProfile(WorkerProfileCreateModel source);
}