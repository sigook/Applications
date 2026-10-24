namespace Covenant.Common.Entities.Accounting.Subcontractors;

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
    public double Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Total { get; private set; }
    public string Description { get; private set; }

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
