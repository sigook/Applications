using System.Threading.Tasks;
using Covenant.IdentityServer.Services.Models;

namespace Covenant.IdentityServer.Services
{
	public interface ITeamsNotification
	{
		Task Send(TeamsNotificationModel notification);
	}
}