namespace Covenant.Common.Models.Accounting.PayStub;

public class CreatePayStubModel
{
    public Guid WorkerProfileId { get; set; }
    public string Position { get; set; }
    public DateTime WorkBegins { get; set; }
    public DateTime WorkEnd { get; set; }
    public decimal OtherDeductions { get; set; }
    public string OtherDeductionsDescription { get; set; }
    public bool PayVacations { get; set; }
    public List<CreatePayStubItemModel> Items { get; set; } = new();
}
