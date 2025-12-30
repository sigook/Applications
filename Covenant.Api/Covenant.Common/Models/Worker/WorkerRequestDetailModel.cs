using Covenant.Common.Models.Location;

namespace Covenant.Common.Models.Worker
{
    public class WorkerRequestDetailModel
    {
        public Guid Id { get; set; }
        public string JobTitle { get; set; }
        public string AgencyFullName { get; set; }
        public string AgencyLogo { get; set; }
        public string Description { get; set; }
        public string Requirements { get; set; }
        public int WorkersQuantity { get; set; }
        public string JobPosition { get; set; }
        public bool HolidayIsPaid { get; set; }
        public bool BreakIsPaid { get; set; }
        public string Status { get; set; }
        public string DurationTerm { get; set; }
        public decimal? Incentive { get; set; }
        public string IncentiveDescription { get; set; }
        public TimeSpan DurationBreak { get; set; }
        public string RequestStatus { get; set; }
        public decimal? WorkerRate { get; set; }
        public decimal? WorkerSalary { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? FinishAt { get; set; }
        public string Location { get; set; }
        public LocationDetailModel JobLocation { get; set; }
        public bool IsApplicant { get; set; }
        public bool PunchCardOptionEnabled { get; set; }
    }
}
