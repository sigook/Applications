using Covenant.Common.Models.Identity;
using Covenant.Common.Models.Locations;

namespace Covenant.Common.Models.Companies;

public class CompanyRegisterByItselfModel : IEmailInformation, IPasswordInformation
{
    public string Name { get; set; }
    public CovenantFileModel Logo { get; set; }
    public string Phone { get; set; }
    public int? PhoneExt { get; set; }
    public ICollection<CompanyProfileJobPositionRateModel> JobPositionRates { get; set; } = new List<CompanyProfileJobPositionRateModel>();
    public ICollection<LocationModel> Locations { get; set; } = new List<LocationModel>();
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}
