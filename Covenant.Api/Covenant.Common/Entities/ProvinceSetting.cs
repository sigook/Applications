namespace Covenant.Common.Entities;

public class ProvinceSetting
{
    public Guid ProvinceId { get; private set; }
    public bool? PaidHolidays { get; private set; }
    public TimeSpan? OvertimeStartsAfter { get; private set; }
    public Province Province { get; private set; }

    internal ProvinceSetting()
    {
    }

    public ProvinceSetting(Guid provinceId, bool? paidHolidays, TimeSpan? overtimeStartsAfter)
    {
        ProvinceId = provinceId;
        PaidHolidays = paidHolidays;
        OvertimeStartsAfter = overtimeStartsAfter;
    }

    public void Update(bool? paidHolidays, TimeSpan? overtimeStartsAfter)
    {
        PaidHolidays = paidHolidays;
        OvertimeStartsAfter = overtimeStartsAfter;
    }
}
