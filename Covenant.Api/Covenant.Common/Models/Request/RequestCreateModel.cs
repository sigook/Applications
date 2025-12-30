using Covenant.Common.Enums;
using Covenant.Common.Models.Location;

namespace Covenant.Common.Models.Request
{
    public class RequestCreateModel
    {
        public string JobTitle { get; set; }
        public string BillingTitle { get; set; }
        public int WorkersQuantity { get; set; }
        public string Description { get; set; }
        public TimeSpan DurationBreak { get; set; }
        public bool BreakIsPaid { get; set; }
        public decimal? Incentive { get; set; }
        public string IncentiveDescription { get; set; }
        public string Requirements { get; set; }
        public string InternalRequirements { get; set; }
        public string Responsibilities { get; set; }
        public bool IsAsap { get; set; }
        public bool JobIsOnBranchOffice { get; set; }
        public LocationDetailModel AnotherLocation { get; set; }
        public Guid? LocationId { get; set; }
        public Guid? JobPositionRateId { get; set; }
        public DurationTerm DurationTerm { get; set; }
        public EmploymentType EmploymentType { get; set; }
        public Guid CompanyProfileId { get; set; }
        public Guid AgencyId { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime? FinishAt { get; set; }
        public ShiftModel Shift { get; set; }
        public bool PunchCardOptionEnabled { get; set; }
        public decimal? WorkerSalary { get; set; }
        public Guid? SalesRepresentativeId { get; set; }
        public IEnumerable<Guid> CompanyUserIds { get; set; }
    }
}