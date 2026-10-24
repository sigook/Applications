using Covenant.Common.Entities;
using Covenant.Common.Entities.Agencies;
using Covenant.Common.Entities.Workers;
using Covenant.Common.Models.Workers;

namespace Covenant.Common.Interfaces.Adapters;

public interface IWorkerAdapter
{
    WorkerProfile MapToWorkerProfile(WorkerProfileCreateModel source, Agency agency, User user);
}