using Covenant.Common.Models.Worker;

namespace Covenant.Common.Entities.Worker
{
    public class WorkerProfileLicense : IWorkerProfileLicense<CovenantFile>
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid WorkerProfileId { get; set; }
        public WorkerProfile WorkerProfile { get; set; }
        public Guid LicenseId { get; set; }
        public CovenantFile License { get; set; }
        public string Number { get; set; }
        public DateTime? Issued { get; set; }
        public DateTime? Expires { get; set; }
    }
}