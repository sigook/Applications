using Covenant.Common.Enums;

namespace Covenant.Common.Models.Company;

public class CompanyProfileDetailModel
{
    public Guid? Id { get; set; }
    public int NumberId { get; set; }
    public Guid CompanyId { get; set; }
    public string FullName { get; set; }
    public string BusinessName { get; set; }
    public string Phone { get; set; }
    public int? PhoneExt { get; set; }
    public string Fax { get; set; }
    public int? FaxExt { get; set; }
    public string Email { get; set; }
    public string Website { get; set; }
    public string About { get; set; }
    public string InternalInfo { get; set; }
    public CompanyStatus CompanyStatus { get; set; }
    public string Password { get; set; }
    public bool Active { get; set; }
    public bool PaidHolidays { get; set; }
    public bool RequiredPaymentMethod { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool? VaccinationRequired { get; set; }
    public string VaccinationRequiredComments { get; set; }
    public bool RequiresPermissionToSeeOrders { get; set; }
    public CovenantFileModel Logo { get; set; }
    public CompanyProfileIndustryDetailModel Industry { get; set; }
    public Guid? SalesRepresentativeId { get; set; }
    public double OvertimeStartsAfter { get; set; }
}