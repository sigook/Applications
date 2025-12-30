namespace Covenant.Common.Entities;

public class ProvinceSetting
{
    public Guid ProvinceId { get; set; }
    public bool? PaidHolidays { get; set; }
    public TimeSpan? OvertimeStartsAfter { get; set; }
    public Province Province { get; set; }
}
