using System.ComponentModel.DataAnnotations;

namespace Covenant.IdentityServer.Controllers.Account.Models
{
	public class ConfirmEmailAddressModel
	{
		[Required] public string Token { get; set; }

		[Required] public string Id { get; set; }
	}
}