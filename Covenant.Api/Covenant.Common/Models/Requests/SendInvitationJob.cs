namespace Covenant.Common.Models.Requests;

public record SendInvitationJob(Guid RequestId, string Nickname);

public record InvitationSentResult(int WorkersSentCount, int CandidatesSentCount, int NumberId, string JobTitle);
