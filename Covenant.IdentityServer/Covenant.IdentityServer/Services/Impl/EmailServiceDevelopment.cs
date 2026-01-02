using System.Threading.Tasks;
using Covenant.IdentityServer.Services.Models;

namespace Covenant.IdentityServer.Services.Impl
{
	public class EmailServiceDevelopment : IEmailService
	{
		private readonly IEmailService _emailService;
		private readonly ITeamsNotification _teamsNotification;

		public EmailServiceDevelopment(IEmailService emailService,ITeamsNotification teamsNotification)
		{
			_emailService = emailService;
			_teamsNotification = teamsNotification;
		}

		public async Task<bool> SendEmail(string email, string subject, string message)
		{
			await _teamsNotification.Send(TeamsNotificationModel.CreateSuccess(subject, message));
			return await _emailService.SendEmail(email, subject, message);
		}
	}
}