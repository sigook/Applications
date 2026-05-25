using Covenant.Common.Enums;

namespace Covenant.Common.Models.Request
{
    public enum GetRequestSortBy : byte
    {
        NumberId,
        Client,
        JobTitle,
        CreatedAt,
        Recruiter,
        Rate,
        WorkersQuantity,
        SalesRepresentative
    }

    public class GetRequestForAgencyFilter : Pagination
    {
        public int? NumberId { get; set; }
        public string CompanyFullName { get; set; }
        public string Location { get; set; }
        public string JobTitle { get; set; }
        public string DisplayRecruiters { get; set; }
        public IEnumerable<RequestStatus> Statuses { get; set; }
        public IEnumerable<Guid> JobBoardIds { get; set; }
        public bool OnlyMine { get; set; }
        public string Recruiter { get; set; }
        public string SalesRepresentative { get; set; }
        public GetRequestSortBy SortBy { get; set; }
        public bool HasPermissionToSeeInternalOrders { get; set; }
        public DateTime? CreatedAtFrom { get; set; }
        public DateTime? CreatedAtTo { get; set; }
        public decimal? RateFrom { get; set; }
        public decimal? RateTo { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? AgencyId { get; set; }
        public string Filter { get; set; }
    }
}