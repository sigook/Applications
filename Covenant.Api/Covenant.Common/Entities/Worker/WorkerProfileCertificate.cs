namespace Covenant.Common.Entities.Worker
{
    public class WorkerProfileCertificate
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid WorkerProfileId { get; set; }
        public WorkerProfile WorkerProfile { get; set; }
        public Guid CertificateId { get; set; }
        public CovenantFile Certificate { get; set; }
    }
}