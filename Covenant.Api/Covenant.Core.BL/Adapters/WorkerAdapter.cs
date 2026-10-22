using Covenant.Common.Entities.Agencies;
using Covenant.Common.Entities.Workers;
using Covenant.Common.Entities;
using Covenant.Common.Interfaces.Adapters;
using Covenant.Common.Models.Workers;

namespace Covenant.Core.BL.Adapters;

public class WorkerAdapter : IWorkerAdapter
{
    public WorkerProfile MapToWorkerProfile(WorkerProfileCreateModel model, Agency agency, User user) =>
        WorkerProfile.Register(model, agency, user);
}
