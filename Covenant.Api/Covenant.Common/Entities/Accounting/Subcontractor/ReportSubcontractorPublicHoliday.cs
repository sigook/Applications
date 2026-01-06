namespace Covenant.Common.Entities.Accounting.Subcontractor;

public class ReportSubcontractorPublicHoliday
{
    public ReportSubcontractorPublicHoliday()
    {
    }

    public ReportSubcontractorPublicHoliday(DateTime holiday, decimal amount)
    {
        Holiday = holiday;
        Amount = amount;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public ReportSubcontractor ReportSubcontractor { get; private set; }
    public Guid ReportSubcontractorId { get; private set; }
    public DateTime Holiday { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }

    public void AssignTo(ReportSubcontractor reportSubcontractor)
    {
        ReportSubcontractor = reportSubcontractor ?? throw new ArgumentNullException();
        ReportSubcontractorId = reportSubcontractor.Id;
    }
}
