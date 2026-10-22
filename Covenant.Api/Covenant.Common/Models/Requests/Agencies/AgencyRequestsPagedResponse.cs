namespace Covenant.Common.Models.Request.Agency;

public class AgencyRequestsPagedResponse : PaginatedList<AgencyRequestListModel>
{
    public IEnumerable<RequestSourceSummaryModel> JobBoardsSummary { get; set; } = new List<RequestSourceSummaryModel>();
}
