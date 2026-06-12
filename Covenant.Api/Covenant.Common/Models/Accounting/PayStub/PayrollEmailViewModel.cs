namespace Covenant.Common.Models.Accounting.PayStub;

public class PayrollEmailViewModel
{
    public string WorkerFullName { get; set; }
    public decimal TotalNet { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime PaymentDate { get; set; }
    public string WorkerEmail { get; set; }
    public string PayrollNumber { get; set; }
}
