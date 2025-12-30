using Covenant.Common.Enums;

namespace Covenant.Common.Models.Request
{
    public enum GetRequestSortBy : byte
    {
        NumberId,
        Client,
        JobTitle,
        StartAt,
        Recruiter,
        Rate,
        WorkersQuantity,
        UpdatedAt,
        SalesRepresentative
    }

    public class GetRequestForAgencyFilter : Pagination
    {
        public int? NumberId { get; set; }
        public string CompanyFullName { get; set; }
        public string JobTitle { get; set; }
        public string DisplayRecruiters { get; set; }
        public IEnumerable<RequestStatus> Statuses { get; set; }
        public bool OnlyMine { get; set; }
        public string Recruiter { get; set; }
        public string SalesRepresentative { get; set; }
        public GetRequestSortBy SortBy { get; set; }
        public bool HasPermissionToSeeInternalOrders { get; set; }
        public DateTime? LastUpdateFrom { get; set; }
        public DateTime? LastUpdateTo { get; set; }
        public DateTime? StartAtFrom { get; set; }
        public DateTime? StartAtTo { get; set; }
        public decimal? RateFrom { get; set; }
        public decimal? RateTo { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? AgencyId { get; set; }
        public string Filter { get; set; }
    }
}