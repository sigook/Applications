using Covenant.Common.Entities.Worker;

namespace Covenant.Common.Entities.Accounting.PayStub;

public class PayStub
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid WorkerProfileId { get; set; }
    public long NumberId { get; set; }
    public WorkerProfile WorkerProfile { get; set; }
    public string PayStubNumber { get; set; }
    public long PayStubNumberId { get; set; }
    public string Position { get; set; }
    public DateTime DateWorkBegins { get; set; }
    public DateTime DateWorkEnd { get; set; }
    public DateTime PaymentDate { get; set; }
    public DateTime WeekEnding { get; set; }
    public decimal RegularWage { get; set; }
    public decimal GrossPayment { get; set; }
    public decimal Vacations { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal Cpp { get; set; }
    public decimal Ei { get; set; }
    public decimal FederalTax { get; set; }
    public decimal ProvincialTax { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal TotalPaid { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<PayStubItem> Items { get; set; } = [];
    public List<PayStubWageDetail> WageDetails { get; set; } = [];
    public List<PayStubPublicHoliday> Holidays { get; set; } = [];
    public List<PayStubOtherDeduction> OtherDeductions { get; set; } = [];
}
