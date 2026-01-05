using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;

namespace Covenant.Common.Models.Request
{
    public class AgencyWorkerRequestModel
    {
        private string _socialInsurance;

        public int NumberId { get; set; }
        public Guid RequestId { get; set; }
        public Guid Id { get; set; }
        public Guid WorkerId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public WorkerRequestStatus WorkerRequestStatus { get; set; }
        public string RejectComments { get; set; }
        public string RejectedBy { get; set; }
        public DateTime? RejectedAt { get; set; }
        public string ProfileImage { get; set; }
        public string Address { get; set; }
        public bool ApprovedToWork { get; set; }
        public bool IsSubcontractor { get; set; }
        public Guid WorkerProfileId { get; set; }
        public string SocialInsurance
        {
            get => WorkerProfile.MaskSINNumber(_socialInsurance);
            set => _socialInsurance = value;
        }
        public DateTime? DueDate { get; set; }
        public bool SocialInsuranceExpire { get; set; }
        public string MobileNumber { get; set; }
        public int NotesCount { get; set; }
        public DateTime? StartWorking { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public double TotalHoursApproved { get; set; }
        public double TotalHoursWorker { get; set; }
    }
}