using System.ComponentModel.DataAnnotations;

namespace Covenant.IdentityServer.Controllers.Account.Models
{
	public class RequestResetPasswordModel
	{
		[Required] [EmailAddress] public string Email { get; set; }
	}
}