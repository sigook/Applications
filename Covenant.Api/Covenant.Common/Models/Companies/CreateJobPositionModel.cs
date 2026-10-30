namespace Covenant.Common.Models.Companies;

public class CreateJobPositionModel
{
    public string JobPosition { get; set; }
    public string Description { get; set; }
    public ShiftModel Shift { get; set; }
}
