using Covenant.Common.Entities.Workers;

namespace Covenant.Common.Entities.Accounting.PayStubs;

public class PayStub
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid WorkerProfileId { get; internal set; }
    public long NumberId { get; internal set; }
    public WorkerProfile WorkerProfile { get; private set; }
    public string PayStubNumber { get; internal set; }
    public long PayStubNumberId { get; internal set; }
    public string Position { get; internal set; }
    public DateTime DateWorkBegins { get; internal set; }
    public DateTime DateWorkEnd { get; internal set; }
    public DateTime PaymentDate { get; internal set; }
    public DateTime WeekEnding { get; internal set; }
    public decimal RegularWage { get; private set; }
    public decimal GrossPayment { get; private set; }
    public decimal Vacations { get; internal set; }
    public decimal TotalEarnings { get; internal set; }
    public decimal Cpp { get; private set; }
    public decimal Ei { get; private set; }
    public decimal FederalTax { get; private set; }
    public decimal ProvincialTax { get; private set; }
    public decimal TotalDeductions { get; private set; }
    public decimal TotalPaid { get; internal set; }
    public DateTime CreatedAt { get; internal set; }
    public List<PayStubItem> Items { get; private set; } = [];
    public List<PayStubWageDetail> WageDetails { get; private set; } = [];
    public List<PayStubPublicHoliday> Holidays { get; private set; } = [];
    public List<PayStubOtherDeduction> OtherDeductions { get; private set; } = [];

    public static PayStub Create(
        Guid workerProfileId,
        long payStubNumberId,
        string payStubNumber,
        string position,
        DateTime dateWorkBegins,
        DateTime dateWorkEnd,
        DateTime paymentDate,
        DateTime weekEnding,
        DateTime createdAt,
        decimal regularWage,
        decimal grossPayment,
        decimal vacations,
        decimal totalEarnings,
        decimal cpp,
        decimal ei,
        decimal federalTax,
        decimal provincialTax,
        decimal totalDeductions,
        decimal totalPaid,
        List<PayStubItem> items,
        List<PayStubOtherDeduction> otherDeductions,
        List<PayStubWageDetail> wageDetails = null,
        List<PayStubPublicHoliday> holidays = null) =>
        new()
        {
            WorkerProfileId = workerProfileId,
            PayStubNumberId = payStubNumberId,
            PayStubNumber = payStubNumber,
            Position = position,
            DateWorkBegins = dateWorkBegins,
            DateWorkEnd = dateWorkEnd,
            PaymentDate = paymentDate,
            WeekEnding = weekEnding,
            CreatedAt = createdAt,
            RegularWage = regularWage,
            GrossPayment = grossPayment,
            Vacations = vacations,
            TotalEarnings = totalEarnings,
            Cpp = cpp,
            Ei = ei,
            FederalTax = federalTax,
            ProvincialTax = provincialTax,
            TotalDeductions = totalDeductions,
            TotalPaid = totalPaid,
            Items = items,
            OtherDeductions = otherDeductions,
            WageDetails = wageDetails ?? [],
            Holidays = holidays ?? []
        };
}
