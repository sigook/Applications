using Covenant.Common.Functionals;
using Covenant.Common.Models.Request.WeeklyBoard;

namespace Covenant.Core.BL.Interfaces
{
    public interface IWeeklyBoardService
    {
        Task<WeeklyBoardModel> GetWeeklyBoard(Guid agencyId, WeeklyBoardFilter filter);
        Task<RecruiterWeeklyBoardModel> GetRecruiterWeeklyBoard(Guid agencyId, WeeklyBoardFilter filter);
        Task<Result> AssignRecruiters(Guid agencyId, AssignRecruitersModel model);
        Task<Result> UnassignRecruiter(Guid agencyId, Guid requestId, Guid recruiterId, DateTime workDate);
        Task<Result> MoveAssignment(Guid agencyId, MoveAssignmentModel model);
        Task<Result> AddWorkers(Guid agencyId, DispatchWorkersModel model);
        Task<Result> RemoveWorker(Guid agencyId, Guid requestId, DateTime workDate, Guid workerProfileId);
    }
}
