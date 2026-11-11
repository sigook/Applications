namespace Covenant.Common.Models.Companies;

public class CompanyJobPositionModel
{
    public Guid? Id { get; set; }
    public string JobPosition { get; set; }
    public double? OvertimeStartsAfter { get; set; }
    public string Description { get; set; }
    public ShiftModel Shift { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string DisplayShift { get; set; }
}
