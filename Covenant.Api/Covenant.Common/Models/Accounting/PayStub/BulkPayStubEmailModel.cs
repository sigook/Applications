namespace Covenant.Common.Models.Accounting.PayStub;

public class BulkPayStubEmailModel
{
    public IEnumerable<Guid> PayStubIds { get; set; }
}
