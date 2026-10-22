using Covenant.Common.Models.Worker;

namespace Covenant.Common.Entities.Worker
{
    public class WorkerProfileLicense : IWorkerProfileLicense<CovenantFile>
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid WorkerProfileId { get; private set; }
        public WorkerProfile WorkerProfile { get; internal set; }
        public Guid LicenseId { get; internal set; }
        public CovenantFile License { get; internal set; }
        public string Number { get; internal set; }
        public DateTime? Issued { get; internal set; }
        public DateTime? Expires { get; internal set; }
    }
}