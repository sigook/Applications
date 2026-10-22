using Covenant.Common.Models.Requests;

namespace Covenant.Core.BL.Interfaces.Requests;

public interface IRequestInvitationService
{
    Task Send(SendInvitationJob job);
}
