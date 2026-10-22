using Covenant.Common.Models.Request;

namespace Covenant.Core.BL.Interfaces.Requests;

public interface IRequestInvitationService
{
    Task Send(SendInvitationJob job);
}
