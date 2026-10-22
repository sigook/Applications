namespace Covenant.Common.Models.Requests.Agencies;

public class BulkRequestRecruiters
{
    public IEnumerable<Guid> Ids { get; set; }
    public IEnumerable<Guid> RecruiterIds { get; set; }
}
