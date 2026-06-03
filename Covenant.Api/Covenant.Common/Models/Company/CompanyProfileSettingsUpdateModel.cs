namespace Covenant.Common.Models.Company;

public class CompanyProfileSettingsUpdateModel
{
    public bool RequiresPermissionToSeeRequests { get; set; }
    public double OvertimeStartsAfter { get; set; }
    public bool PaidHolidays { get; set; }
}
