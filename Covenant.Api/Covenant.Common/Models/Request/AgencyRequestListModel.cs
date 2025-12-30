using Covenant.Common.Enums;

namespace Covenant.Common.Models.Request
{
    public class AgencyRequestListModel
    {
        public Guid Id { get; set; }
        public Guid AgencyId { get; set; }
        public int NumberId { get; set; }
        public string JobTitle { get; set; }
        public string BillingTitle { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? FinishAt { get; set; }
        public DateTime? StartAt { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ProvinceCode { get; set; }
        public string PostalCode { get; set; }
        public string Entrance { get; set; }
        public string CompanyFullName { get; set; }
        public Guid CompanyProfileId { get; set; }
        public string Status { get; set; }
        public RequestStatus RequestStatus { get; set; }
        public string DurationTerm { get; set; }
        public string EmploymentType { get; set; }
        public int WorkersQuantity { get; set; }
        public int WorkersQuantityWorking { get; set; }
        public bool IsOpen { get; set; }
        public bool IsAsap { get; set; }
        public decimal? WorkerRate { get; set; }
        public decimal? WorkerSalary { get; set; }
        public string DisplayRecruiters { get; set; }
        public string DisplayShift { get; set; }
        public string SalesRepresentative { get; set; }
        public int NotesCount { get; set; }
        public bool VaccinationRequired { get; set; }
        public bool PunchCardOptionEnabled { get; set; }
        public bool HasPermissionToSeeInternalOrders { get; set; }

        public string Location => $"{Address} {City} {ProvinceCode} {PostalCode}";
    }
}