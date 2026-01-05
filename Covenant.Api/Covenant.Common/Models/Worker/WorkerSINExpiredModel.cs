namespace Covenant.Common.Models.Worker
{
    public class WorkerSINExpiredModel
    {
        public string WorkerFullName { get; set; }
        public string SocialInsurance { get; set; }
        public DateTime? DueDate { get; set; }
        public string MobileNumber { get; set; }
        public string Phone { get; set; }
        public int? PhoneExt { get; set; }
        public string WorkerEmail { get; set; }
        public string AgencyEmail { get; set; }
        public string RecruitmentEmail { get; set; }
    }
}