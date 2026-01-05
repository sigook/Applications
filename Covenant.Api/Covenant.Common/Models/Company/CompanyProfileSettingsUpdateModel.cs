namespace Covenant.Common.Models.Company;

public class CompanyProfileSettingsUpdateModel
{
    public bool RequiresPermissionToSeeOrders { get; set; }
    public double OvertimeStartsAfter { get; set; }
    public bool PaidHolidays { get; set; }
}
