using Covenant.Common.Functionals;
using Covenant.Common.Models.Request.Runners;

namespace Covenant.Core.BL.Interfaces;

public interface IRunnerService
{
    Task<Result<Guid>> CreateRunner(Guid requestId, RunnerCreateModel model, Guid? requestRecruiterId = null);
    Task<Result> DeleteRunner(Guid runnerId);
    Task<Result> ChangeStatus(Guid runnerId, ChangeRunnerStatusModel model);
    Task<Result<Guid>> AddInterview(Guid runnerId, RunnerInterviewCreateModel model);
    Task<Result> RescheduleInterview(Guid runnerId, Guid interviewId, RunnerInterviewRescheduleModel model);
    Task<List<RunnerStartingTodayModel>> GetRunnersStartingToday();
}
