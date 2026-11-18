namespace Covenant.Common.Entities;

public class ProvinceSetting
{
    public Guid ProvinceId { get; private set; }
    public TimeSpan? OvertimeStartsAfter { get; private set; }
    public Province Province { get; private set; }

    internal ProvinceSetting()
    {
    }

    public ProvinceSetting(Guid provinceId, TimeSpan? overtimeStartsAfter)
    {
        ProvinceId = provinceId;
        OvertimeStartsAfter = overtimeStartsAfter;
    }

    public void Update(TimeSpan? overtimeStartsAfter)
    {
        OvertimeStartsAfter = overtimeStartsAfter;
    }
}
