using Covenant.Common.Entities.Request;
using Covenant.Common.Models.Candidate;
using Covenant.Common.Models.Website;

namespace Covenant.Core.BL.Interfaces.Candidates;

public interface ICandidateApplicationService
{
    Task ProcessApplication(CandidateViewModel message);
    Task RegisterApplicant(RequestApplicant applicant);
    Task NotifyFailure(string error);
    Task NotifyApplication(RequestApplicant applicant);
    Task NotifyCreation(CandidateCreateModel model);
}
