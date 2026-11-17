using Covenant.Common.Entities.Requests;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Candidates;
using Covenant.Common.Models.Website;

namespace Covenant.Core.BL.Interfaces.Candidates;

public interface ICandidateApplicationService
{
    Task<Result> SubmitApplication(CandidateViewModel candidate, Stream resume, string resumeFileName);
    Task ProcessApplication(CandidateViewModel message);
    Task RegisterApplicant(RequestApplicant applicant);
    Task NotifyFailure(string error);
    Task NotifyApplication(RequestApplicant applicant);
    Task NotifyCreation(CandidateCreateModel model);
}
