using Covenant.Common.Entities.Candidate;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Core.BL.Interfaces;

namespace Covenant.Core.BL.Services;

public class RequestApplicantNotificationService(ISendGridService sendGridService) : IRequestApplicantNotificationService
{
    private const string Template = "NEW_APPLICANT";

    public Task Notify(Request request, WorkerProfile workerProfile) =>
        Send(request, new
        {
            RequestNumberId = request.NumberId,
            request.JobTitle,
            WorkerNumberId = workerProfile.NumberId,
            Nmae = workerProfile.FullName,
            Email = workerProfile.Worker?.Email,
            Phone = $"{workerProfile.Phone}",
            workerProfile.MobileNumber,
            workerProfile.Location?.FormattedAddress,
            Skills = string.Join(",", workerProfile.Skills?.Select(s => s.Skill) ?? []),
            Sin = workerProfile.MaskedSocialInsurance,
            SinExpire = workerProfile.DueDate?.ToString("D")
        });

    public Task Notify(Request request, Candidate candidate) =>
        Send(request, new
        {
            RequestNumberId = request.NumberId,
            request.JobTitle,
            CandidateNumberId = candidate.NumberId,
            Nmae = candidate.Name,
            candidate.Email,
            Phone = candidate.PhoneNumbers?.FirstOrDefault()?.PhoneNumber,
            candidate.Address,
            Skills = string.Join(",", candidate.Skills?.Select(s => s.Skill) ?? [])
        });

    private async Task Send(Request request, object data)
    {
        if (!request.Recruiters.Any()) return;
        await sendGridService.SendEmail(new SendGridModel
        {
            Tos = request.Recruiters.Select(r => r.Recruiter.User.Email),
            Template = Template,
            Data = data
        });
    }
}
