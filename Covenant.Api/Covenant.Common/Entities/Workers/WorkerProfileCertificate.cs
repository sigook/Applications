namespace Covenant.Common.Entities.Worker
{
    public class WorkerProfileCertificate
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid WorkerProfileId { get; private set; }
        public WorkerProfile WorkerProfile { get; internal set; }
        public Guid CertificateId { get; internal set; }
        public CovenantFile Certificate { get; internal set; }
    }
}