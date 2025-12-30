using Covenant.Common.Entities.Worker;

namespace Covenant.Common.Entities.Accounting.Subcontractor;

public class ReportSubcontractor
{
    public ReportSubcontractor()
    {
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid WorkerProfileId { get; set; }
    public WorkerProfile WorkerProfile { get; private set; }
    public decimal RegularWage { get; set; }
    public decimal PublicHolidayPay { get; set; }
    public decimal Gross { get; set; }
    public decimal Earnings { get; set; }
    public decimal TotalNet { get; set; }
    public decimal DeductionOthers { get; private set; }
    public DateTime DateWorkBegins { get; set; }
    public DateTime DateWorkEnd { get; set; }
    public DateTime WeekEnding { get; set; }
    public long NumberId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public IEnumerable<ReportSubcontractorWageDetail> WageDetails { get; set; } = new List<ReportSubcontractorWageDetail>();
    public IEnumerable<ReportSubcontractorPublicHoliday> Holidays { get; set; } = new List<ReportSubcontractorPublicHoliday>();
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
}
