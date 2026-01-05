namespace Covenant.Common.Entities.Accounting.Subcontractor;

public class ReportSubContractorOtherDeduction
{
    public ReportSubContractorOtherDeduction()
    {
    }

    public ReportSubContractorOtherDeduction(double quantity, decimal unitPrice, string description, Guid id = default)
    {
        Id = id == default ? Guid.NewGuid() : id;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Description = description;
        Total = decimal.Multiply(new decimal(quantity), unitPrice);
    }

    public Guid Id { get; private set; }
    public double Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
    public string Description { get; set; }

    public Guid ReportSubcontractorId { get; private set; }
    public ReportSubcontractor ReportSubcontractor { get; private set; }

    public void AssignTo(ReportSubcontractor reportSubcontractor)
    {
        ReportSubcontractor = reportSubcontractor ?? throw new ArgumentNullException(nameof(reportSubcontractor));
        ReportSubcontractorId = reportSubcontractor.Id;
    }

    public static ReportSubContractorOtherDeduction CreateDefaultDeduction(decimal total, string description) =>
        new ReportSubContractorOtherDeduction(1, total, description);
}
