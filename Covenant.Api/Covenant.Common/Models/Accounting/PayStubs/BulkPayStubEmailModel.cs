namespace Covenant.Common.Models.Accounting.PayStubs;

public class BulkPayStubEmailModel
{
    public IEnumerable<Guid> PayStubIds { get; set; }
}
