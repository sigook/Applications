using Covenant.Common.Models.Location;

namespace Covenant.Common.Models.Request
{
    public class AgencyRequestDetailModel
    {
        public Guid Id { get; set; }
        public int NumberId { get; set; }
        public string JobTitle { get; set; }
        public string BillingTitle { get; set; }
        public string CompanyLogo { get; set; }
        public string FullName { get; set; }
        public Guid CompanyProfileId { get; set; }
        public string Description { get; set; }
        public string Requirements { get; set; }
        public string Responsibilities { get; set; }
        public LocationDetailModel JobLocation { get; set; }
        public int WorkersQuantity { get; set; }
        public int WorkersQuantityWorking { get; set; }
        public Guid? JobPositionId { get; set; }
        public string JobPosition { get; set; }
        public bool HolidayIsPaid { get; set; }
        public bool BreakIsPaid { get; set; }
        public string Status { get; set; }
        public string CancellationDetail { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? FinishAt { get; set; }
        public DateTime? InvitationSentItAt { get; set; }
        public TimeSpan DurationBreak { get; set; }
        public decimal? Incentive { get; set; }
        public string IncentiveDescription { get; set; }
        public decimal? AgencyRate { get; set; }
        public decimal? WorkerRate { get; set; }
        public decimal? WorkerSalary { get; set; }
        public string DurationTerm { get; set; }
        public string EmploymentType { get; set; }
        public string DisplayRecruiters { get; set; }
        public string DisplayShift { get; set; }
        public bool IsAsap { get; set; }
        public bool? VaccinationRequired { get; set; }
        public bool PunchCardOptionEnabled { get; set; }
        public string InternalRequirements { get; set; }
        public Guid? SalesRepresentativeId { get; set; }
        public IEnumerable<Guid> CompanyUserIds { get; set; }
    }
}