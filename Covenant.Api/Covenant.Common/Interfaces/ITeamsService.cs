using Covenant.Common.Functionals;
using Covenant.Common.Models.Notifications;
using Covenant.Common.Models.Teams;

namespace Covenant.Common.Interfaces;

public interface ITeamsService
{
    Task<Result> SendNotification(string webhook, TeamsNotificationModel notification);

    Task<Stream> GetTeamsFile(string url);

    Task<Result> ExistsTeamsFile(string url);

    Task<Result<IReadOnlyList<TeamsChannelFileModel>>> FindChannelFiles(TeamsChannelFileFilter filter);

    Task<Result<Stream>> GetChannelFile(TeamsChannelFileFilter filter);
}
