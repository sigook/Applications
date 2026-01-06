using Covenant.Common.Functionals;
using Covenant.Common.Models.Notification;

namespace Covenant.Common.Interfaces
{
    public interface ITeamsService
    {
        Task<Result> SendNotification(string webhook, TeamsNotificationModel notification);

        Task<Stream> GetTeamsFile(string url);

        Task<Result> ExistsTeamsFile(string url);
    }
}