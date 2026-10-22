namespace Covenant.Common.Models.Requests.Agencies;

public class AgencyRequestsPagedResponse : PaginatedList<AgencyRequestListModel>
{
    public IEnumerable<RequestSourceSummaryModel> JobBoardsSummary { get; set; } = new List<RequestSourceSummaryModel>();
}
