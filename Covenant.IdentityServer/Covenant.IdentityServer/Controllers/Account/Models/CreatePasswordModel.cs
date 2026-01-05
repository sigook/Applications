using System.ComponentModel.DataAnnotations;

namespace Covenant.IdentityServer.Controllers.Account.Models
{
	public class CreatePasswordModel
	{
		[Required(ErrorMessage = "Please request an email to get a token")]
		public string Token { get; set; }

		[Required(ErrorMessage = "Please request an email to get an id")]
		public string Id { get; set; }

		[Required, DataType(DataType.Password)]
		public string Password { get; set; }

		[Required, Compare(nameof(Password), ErrorMessage = "Password and confirm password do not match")]
		public string ConfirmPassword { get; set; }

		public bool InvalidToken { get; set; }
	}
}