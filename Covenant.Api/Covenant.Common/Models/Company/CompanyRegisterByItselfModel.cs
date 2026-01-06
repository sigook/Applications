using Covenant.Common.Models.Location;
using Covenant.Common.Models.Security;

namespace Covenant.Common.Models.Company
{
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
}
