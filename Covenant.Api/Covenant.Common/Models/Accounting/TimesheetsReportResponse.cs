namespace Covenant.Common.Models.Accounting;

public class TimesheetsReportResponse
{
    public int EmployeeId { get; set; }
    public string FullName { get; set; }
    public string SocialInsurance { get; set; }
    public string WcCode { get; set; }
    public string Client { get; set; }
    public string JobName { get; set; }
    public decimal PayRate { get; set; }
    public double RegularHours { get; set; }
    public double OvertimeHours { get; set; }
}
