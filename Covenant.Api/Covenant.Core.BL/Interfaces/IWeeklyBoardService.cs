using Covenant.Common.Functionals;
using Covenant.Common.Models.Request.WeeklyBoard;

namespace Covenant.Core.BL.Interfaces
{
    public interface IWeeklyBoardService
    {
        Task<WeeklyBoardModel> GetWeeklyBoard(WeeklyBoardFilter filter);
        Task<RecruiterWeeklyBoardModel> GetRecruiterWeeklyBoard(WeeklyBoardFilter filter);
        Task<IEnumerable<WeeklyBoardRunnerModel>> GetOrderRunners(Guid requestId);
        Task<Result> AssignRecruiters(AssignRecruitersModel model);
        Task<Result> UnassignRecruiter(Guid requestId, Guid recruiterId, DateTime workDate);
        Task<Result> MoveAssignment(MoveAssignmentModel model);
        Task<Result> AddRunner(AddRunnerModel model);
    }
}
