using Covenant.Common.Entities.Workers;

namespace Covenant.Common.Entities.Accounting.Subcontractors;

public class ReportSubcontractor
{
    public ReportSubcontractor()
    {
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid WorkerProfileId { get; internal set; }
    public WorkerProfile WorkerProfile { get; private set; }
    public decimal RegularWage { get; internal set; }
    public decimal PublicHolidayPay { get; private set; }
    public decimal Gross { get; internal set; }
    public decimal Earnings { get; internal set; }
    public decimal TotalNet { get; internal set; }
    public decimal DeductionOthers { get; private set; }
    public DateTime DateWorkBegins { get; internal set; }
    public DateTime DateWorkEnd { get; internal set; }
    public DateTime WeekEnding { get; internal set; }
    public long NumberId { get; internal set; }
    public DateTime CreatedAt { get; private set; } = DateTime.Now;
    public IEnumerable<ReportSubcontractorWageDetail> WageDetails { get; private set; } = new List<ReportSubcontractorWageDetail>();
    public IEnumerable<ReportSubcontractorPublicHoliday> Holidays { get; private set; } = new List<ReportSubcontractorPublicHoliday>();
    public IEnumerable<ReportSubContractorOtherDeduction> OtherDeductionsDetail { get; private set; } = new List<ReportSubContractorOtherDeduction>();
    public decimal TotalOvertime => WageDetails.Sum(c => c.Overtime);
    public decimal TotalRegular => WageDetails.Sum(c => c.Regular);

    public void AddWageDetail(IEnumerable<ReportSubcontractorWageDetail> wageDetails)
    {
        WageDetails = new List<ReportSubcontractorWageDetail>(wageDetails);
        foreach (ReportSubcontractorWageDetail detail in WageDetails) detail.AssignTo(this);
    }

    public void AddHolidays(IEnumerable<ReportSubcontractorPublicHoliday> holidays)
    {
        Holidays = new List<ReportSubcontractorPublicHoliday>(holidays);
        foreach (ReportSubcontractorPublicHoliday item in Holidays) item.AssignTo(this);
    }

    public void AddOtherDeductionsDetail(IEnumerable<ReportSubContractorOtherDeduction> otherDeductions)
    {
        var list = new List<ReportSubContractorOtherDeduction>();
        decimal totalOtherDeductions = 0;
        foreach (ReportSubContractorOtherDeduction deduction in otherDeductions)
        {
            if (deduction.Total <= 0) continue;
            deduction.AssignTo(this);
            totalOtherDeductions += deduction.Total;
            list.Add(deduction);
        }
        OtherDeductionsDetail = new List<ReportSubContractorOtherDeduction>(list);
        DeductionOthers = totalOtherDeductions;
    }

    public static ReportSubcontractor Create(Guid workerProfileId, decimal regularWage, decimal gross, DateTime dateWorkBegins, DateTime dateWorkEnd, DateTime weekEnding) =>
        new()
        {
            WorkerProfileId = workerProfileId,
            RegularWage = regularWage,
            Gross = gross,
            PublicHolidayPay = 0,
            Earnings = gross,
            TotalNet = gross,
            DateWorkBegins = dateWorkBegins,
            DateWorkEnd = dateWorkEnd,
            WeekEnding = weekEnding
        };
}
