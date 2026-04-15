using Covenant.Common.Entities;
using Covenant.Common.Entities.Agency;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Models.Worker;

namespace Covenant.Common.Interfaces.Adapters;

public interface IWorkerAdapter
{
    WorkerProfile MapToWorkerProfile(WorkerProfileCreateModel source, Agency agency, User user);
}