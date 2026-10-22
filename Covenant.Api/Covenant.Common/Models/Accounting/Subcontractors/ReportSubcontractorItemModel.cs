namespace Covenant.Common.Models.Accounting.Subcontractor;

public class ReportSubcontractorItemModel
{
	public string Company { get; set; }
	public decimal WorkerRate { get; set; }
	public decimal Regular { get;  set; }
	public decimal OtherRegular { get; set; }
	public decimal Overtime { get;  set; }
	public decimal Holiday { get; set; }
	public decimal Missing { get; set; }
	public decimal MissingOvertime { get; set; }
	public double RegularHours { get; set; }
	public double OtherRegularHours { get; set; }
	public double OvertimeHours { get; set; }
	public double HolidayHours { get; set; }
	public double MissingHours { get; set; }
	public double MissingOvertimeHours { get; set; }
	public decimal Others { get; set; }
}