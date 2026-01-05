using Covenant.Common.Models.Location;

namespace Covenant.Common.Models.Request
{
    public class CompanyRequestDetailModel
    {
        public Guid Id { get; set; }
        public string JobTitle { get; set; }
        public int WorkersQuantity { get; set; }
        public int WorkersQuantityWorking { get; set; }
        public string Description { get; set; }
        public string Requirements { get; set; }
        public string Responsibilities { get; set; }
        public TimeSpan DurationBreak { get; set; }
        public bool BreakIsPaid { get; set; }
        public bool HolidayIsPaid { get; set; }
        public decimal? Incentive { get; set; }
        public string IncentiveDescription { get; set; }
        public bool IsAsap { get; set; }
        public bool JobIsOnBranchOffice { get; set; }
        public LocationDetailModel JobLocation { get; set; }
        public JobPositionDetailModel JobPositionRate { get; set; }
        public decimal? AgencyRate { get; set; }
        public string Status { get; set; }
        public string DurationTerm { get; set; }
        public string DisplayShift { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? FinishAt { get; set; }
        public decimal? WorkerSalary { get; set; }
    }
}