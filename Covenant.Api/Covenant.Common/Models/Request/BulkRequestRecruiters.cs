namespace Covenant.Common.Models.Request;

public class BulkRequestRecruiters
{
    public IEnumerable<Guid> Ids { get; set; }
    public IEnumerable<Guid> RecruiterIds { get; set; }
}
