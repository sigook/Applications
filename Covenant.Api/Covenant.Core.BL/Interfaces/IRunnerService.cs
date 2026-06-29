using Covenant.Common.Functionals;
using Covenant.Common.Models.Request.Runners;

namespace Covenant.Core.BL.Interfaces;

public interface IRunnerService
{
    Task<Result<Guid>> CreateRunner(Guid agencyId, Guid requestId, RunnerCreateModel model, string createdBy);
    Task<Result> ChangeStatus(Guid runnerId, ChangeRunnerStatusModel model, string changedBy);
    Task<Result<Guid>> AddInterview(Guid runnerId, RunnerInterviewCreateModel model, string createdBy);
    Task<Result> RescheduleInterview(Guid runnerId, Guid interviewId, RunnerInterviewRescheduleModel model, string rescheduledBy);
}
