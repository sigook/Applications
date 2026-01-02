using System.Threading.Tasks;
using Covenant.IdentityServer.Services;

namespace Covenant.IdentityServer.Tests.Utils
{
	internal class FakeEmailService:IEmailService
	{
		public string Email { get; set; }
		public string Subject { get; set; }
		public string Message { get; set; }
		public Task<bool> SendEmail(string email, string subject, string message)
		{
			Email = email;
			Subject = subject;
			Message = message;
			return Task.FromResult(true);
		}
	}
}