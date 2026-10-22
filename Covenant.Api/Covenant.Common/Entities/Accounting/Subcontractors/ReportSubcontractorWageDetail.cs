using Covenant.Common.Entities.Request;

namespace Covenant.Common.Entities.Accounting.Subcontractor;

public class ReportSubcontractorWageDetail
{
    public ReportSubcontractorWageDetail()
    {
    }

    public ReportSubcontractorWageDetail(
        decimal workerRate,
        decimal regular,
        decimal otherRegular,
        decimal missing,
        decimal missingOvertime,
        decimal nightShift,
        decimal holiday,
        decimal overtime)
    {
        WorkerRate = workerRate;
        Regular = regular;
        OtherRegular = otherRegular;
        Missing = missing;
        MissingOvertime = missingOvertime;
        NightShift = nightShift;
        Holiday = holiday;
        Overtime = overtime;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public decimal WorkerRate { get; private set; }
    public decimal Regular { get; private set; }
    public decimal OtherRegular { get; private set; }
    public decimal Missing { get; private set; }
    public decimal NightShift { get; private set; }
    public decimal Holiday { get; private set; }
    public decimal Overtime { get; private set; }
    public decimal MissingOvertime { get; private set; }
    public decimal Gross => Regular + OtherRegular + Missing + MissingOvertime + NightShift + Holiday + Overtime;
    public Guid TimeSheetTotalId { get; private set; }
    public TimeSheetTotalPayroll TimeSheetTotal { get; internal set; }
    public ReportSubcontractor ReportSubcontractor { get; private set; }
    public Guid ReportSubcontractorId { get; private set; }

    public void AssignTo(ReportSubcontractor reportSubcontractor)
    {
        ReportSubcontractor = reportSubcontractor ?? throw new ArgumentNullException();
        ReportSubcontractorId = reportSubcontractor.Id;
    }

    public ReportSubcontractorWageDetail AttachTimeSheetTotal(TimeSheetTotalPayroll timeSheetTotal)
    {
        TimeSheetTotal = timeSheetTotal;
        return this;
    }
}
