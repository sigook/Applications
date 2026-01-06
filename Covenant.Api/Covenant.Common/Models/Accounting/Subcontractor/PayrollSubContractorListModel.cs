namespace Covenant.Common.Models.Accounting.Subcontractor;

public class PayrollSubContractorListModel
{
    public decimal TotalNet { get; set; }
    public DateTime WeekEnding { get; set; }
    public int NumberOfWorkers { get; set; }
    public string WeekEndingDisplay => WeekEnding.ToString("yyyy-MM-dd");
}