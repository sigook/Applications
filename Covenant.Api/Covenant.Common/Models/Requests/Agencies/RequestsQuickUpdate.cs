namespace Covenant.Common.Models.Request.Agency;

public class RequestsQuickUpdate
{
    public bool IsAsap { get; set; }
    public IEnumerable<Guid> Ids { get; set; }
}
