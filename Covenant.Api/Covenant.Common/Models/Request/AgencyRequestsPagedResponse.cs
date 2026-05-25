namespace Covenant.Common.Models.Request
{
    public class AgencyRequestsPagedResponse : PaginatedList<AgencyRequestListModel>
    {
        public IEnumerable<RequestSourceSummaryModel> JobBoardsSummary { get; set; } = new List<RequestSourceSummaryModel>();
    }
}
