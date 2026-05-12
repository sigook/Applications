using Covenant.Common.Models.Agency;
using Covenant.Common.Models.Worker;

namespace Covenant.Common.Interfaces;

public interface IInvitationEmailService
{
    Task SendInvitations(InvitationToApplyModel invitation, IReadOnlyCollection<WorkerContactInfoModel> workers);
}
