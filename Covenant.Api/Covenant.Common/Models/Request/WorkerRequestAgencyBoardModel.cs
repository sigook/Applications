using Covenant.Common.Entities.Worker;

namespace Covenant.Common.Models.Request
{
    public class WorkerRequestAgencyBoardModel
    {
        public Guid Id { get; set; }
        public DateTime? StartWorking { get; set; }
        public DateTime? WeekStartWorking { get; set; }
        public string WorkerRequestStatus { get; set; }
        public string RejectComments { get; set; }
        public DateTime? RejectedAt { get; set; }
        public Guid RequestId { get; set; }
        public int NumberId { get; set; }
        public string RequestStatus { get; set; }
        public string JobTitle { get; set; }
        public decimal? WorkerRate { get; set; }
        public decimal? WorkerSalary { get; set; }
        public string DurationTerm { get; set; }
        public string DisplayRecruiters { get; set; }
        public string Location { get; set; }
        public string Entrance { get; set; }
        public string DisplayShift { get; set; }
        public Guid WorkerProfileId { get; set; }
        public Guid WorkerId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string SecondLastName { get; set; }
        private string _socialInsurance;
        public string SocialInsurance
        {
            get => WorkerProfile.MaskSINNumber(_socialInsurance);
            set => _socialInsurance = value;
        }
        public bool SocialInsuranceExpire { get; set; }
        public DateTime? DueDate { get; set; }
        public string MobileNumber { get; set; }
        public bool IsSubcontractor { get; set; }
        public Guid CompanyProfileId { get; set; }
        public string CompanyFullName { get; set; }
        public int NotesCount { get; set; }
    }
}