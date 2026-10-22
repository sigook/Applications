using Covenant.Common.Entities;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Interfaces.Adapters;
using Covenant.Common.Models.Worker;

namespace Covenant.Core.BL.Adapters;

public class WorkerAdapter : IWorkerAdapter
{
    public WorkerProfile MapToWorkerProfile(WorkerProfileCreateModel model, Common.Entities.Agency.Agency agency, User user) =>
        WorkerProfile.Register(model, agency, user);
}
