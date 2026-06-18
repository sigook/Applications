namespace Covenant.Common.Models.Request;

public record SendInvitationJob(Guid RequestId, string Nickname);

public record InvitationSentResult(int SentCount, int NumberId, string JobTitle);
