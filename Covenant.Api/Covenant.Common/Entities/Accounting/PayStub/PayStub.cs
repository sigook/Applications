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
    public string TypeOfWork { get; set; }
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
    public decimal OtherDeductions { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal TotalPaid { get; set; }
    public DateTime CreatedAt { get; set; }
    public IEnumerable<PayStubItem> Items { get; set; } = new List<PayStubItem>();
    public IEnumerable<PayStubWageDetail> WageDetails { get; set; } = new List<PayStubWageDetail>();
    public IEnumerable<PayStubPublicHoliday> Holidays { get; set; } = new List<PayStubPublicHoliday>();
    public IEnumerable<PayStubOtherDeduction> OtherDeductionsDetail { get; set; } = new List<PayStubOtherDeduction>();

    public void AddItems(IEnumerable<PayStubItem> items)
    {
        Items = [.. items];
        foreach (var item in Items) item.AssignTo(this);
    }

    public void AddWageDetails(IEnumerable<PayStubWageDetail> wageDetails)
    {
        WageDetails = [.. wageDetails];
        foreach (var wageDetail in WageDetails) wageDetail.AssignTo(this);
    }

    public void AddHolidays(IEnumerable<PayStubPublicHoliday> holidays)
    {
        Holidays = [.. holidays];
        foreach (var holiday in Holidays) holiday.AssignTo(this);
    }

    public void AddOtherDeductionsDetail(IEnumerable<PayStubOtherDeduction> otherDeductions)
    {
        var list = new List<PayStubOtherDeduction>();
        decimal totalOtherDeductions = 0;
        foreach (var deduction in otherDeductions)
        {
            if (deduction.Total <= 0) continue;
            deduction.AssignTo(this);
            totalOtherDeductions += deduction.Total;
            list.Add(deduction);
        }
        OtherDeductionsDetail = [.. list];
        OtherDeductions = totalOtherDeductions;
    }

}
