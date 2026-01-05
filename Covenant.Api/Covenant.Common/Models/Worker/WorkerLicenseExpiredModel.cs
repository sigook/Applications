namespace Covenant.Common.Models.Worker
{
    public class WorkerLicenseExpiredModel
    {
        public long NumberId { get; set; }
        public string WorkerFullName { get; set; }
        public string WorkerEmail { get; set; }
        public string MobileNumber { get; set; }
        public string LicenseDescription { get; set; }
        public string LicenseNumber { get; set; }
        public DateTime? Expires { get; set; }
        public string AgencyEmail { get; set; }
        public string RecruitmentEmail { get; set; }
    }
}