using System.Text.Json.Serialization;

namespace Covenant.Common.Models.Accounting.PayStubs;

public class WorkerPayStubListModel
{
    public Guid Id { get; set; }
    public string PayStubNumber { get; set; }
    public DateTime WeekEnding { get; set; }
    public DateTime PaymentDate { get; set; }
    public double Hours { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal TotalPaid { get; set; }
    public IEnumerable<string> Companies { get; set; } = [];
    [JsonIgnore]
    public DateTime? PdfGeneratedAt { get; set; }
    public string PdfUrl { get; set; }
}
