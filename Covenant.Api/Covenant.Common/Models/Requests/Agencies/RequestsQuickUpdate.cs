namespace Covenant.Common.Models.Requests.Agencies;

public class RequestsQuickUpdate
{
    public bool IsAsap { get; set; }
    public IEnumerable<Guid> Ids { get; set; }
}
