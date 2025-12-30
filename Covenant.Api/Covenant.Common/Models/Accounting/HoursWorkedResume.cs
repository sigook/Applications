namespace Covenant.Common.Models.Accounting;

public class HoursWorkedResume
{
    public double TotalRegularHours { get; set; }
    public double TotalOvertimeHours { get; set; }
    public double TotalHolidayHours { get; set; }
    public double TotalNightHours { get; set; }
    public double TotalHours { get; set; }
    public decimal TotalPayRegular { get; set; }
    public decimal TotalPayOvertime { get; set; }
    public decimal TotalPayHoliday { get; set; }
    public decimal TotalPayNight { get; set; }
    public decimal TotalPay { get; set; }
    public IEnumerable<HoursWorkedResponse> Detail { get; set; }
}
