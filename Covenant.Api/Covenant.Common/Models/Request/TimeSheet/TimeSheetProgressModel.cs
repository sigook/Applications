using Covenant.Common.Enums;

namespace Covenant.Common.Models.Request.TimeSheet
{
    public class TimeSheetProgressModel
    {
        public Guid WorkerId { get; set; }
        public Guid WorkerProfileId { get; set; }
        public Guid RequestId { get; set; }
        public bool IsSubcontractor { get; set; }
        public double TotalHoursWorker { get; set; }
        public string WorkerName { get; set; }
        public WorkerRequestStatus WorkerRequestStatus { get; set; }
        public string Status { get; set; }
        public double TotalHoursApproved { get; set; }
        public int NumberId { get; set; }
        public string ProfileImage { get; set; }
    }
}