using Covenant.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Covenant.Common.Models.Security
{
    public class CreateUserModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public UserType UserType { get; set; }
        public Guid? AgencyId { get; set; }
        public Guid? CompanyId { get; set; }
    }
}
