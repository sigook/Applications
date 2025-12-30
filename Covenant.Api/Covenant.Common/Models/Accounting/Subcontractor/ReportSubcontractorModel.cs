namespace Covenant.Common.Models.Accounting.Subcontractor;

public class ReportSubcontractorModel
{
    public string FullName { get; set; }
    public List<ReportSubcontractorItemModel> Items { get; set; } = new List<ReportSubcontractorItemModel>();
    public decimal Deductions { get; set; }
    public decimal PublicHoliday { get; set; }
    public decimal TotalNet { get; set; }
    public string Email { get; set; }
    public DateTime WeekEnding { get; set; }
    public List<ReportSubcontractorItemModel> ItemsGroupByWorkerRate
    {
        get
        {
            return Items.GroupBy(m => m.WorkerRate)
                .Select(g => new ReportSubcontractorItemModel
                {
                    WorkerRate = g.Key,
                    Company = g.First().Company,
                    Regular = g.Sum(r => r.Regular),
                    OtherRegular = g.Sum(r => r.OtherRegular),
                    RegularHours = g.Sum(rh => rh.RegularHours),
                    OtherRegularHours = g.Sum(rh => rh.OtherRegularHours),
                    Overtime = g.Sum(o => o.Overtime),
                    OvertimeHours = g.Sum(oh => oh.OvertimeHours),
                    Holiday = g.Sum(h => h.Holiday),
                    HolidayHours = g.Sum(hh => hh.HolidayHours),
                    Missing = g.Sum(m => m.Missing),
                    MissingHours = g.Sum(mh => mh.MissingHours),
                    MissingOvertime = g.Sum(mo => mo.MissingOvertime),
                    MissingOvertimeHours = g.Sum(moh => moh.MissingOvertimeHours),
                    Others = g.Sum(o => o.Others)
                }).ToList();
        }
    }
}