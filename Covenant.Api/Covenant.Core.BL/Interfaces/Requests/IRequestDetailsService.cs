using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
using Covenant.Common.Models.Request.Agency;

namespace Covenant.Core.BL.Interfaces.Requests;

public interface IRequestDetailsService
{
    Task<Result<AgencyRequestDetailModel>> UpdateShift(Guid requestId, ShiftModel model);
    Task<Result> AddReportTo(Guid requestId, Guid contactPersonId);
    Task<Result> DeleteReportTo(Guid requestId, Guid contactPersonId);
    Task<Result> AddRequestedBy(Guid requestId, Guid contactPersonId);
    Task<Result> DeleteRequestedBy(Guid requestId, Guid contactPersonId);
    Task<Result<SkillModel>> AddSkill(Guid requestId, string skill);
    Task<Result> DeleteSkill(Guid requestId, Guid skillId);
    Task<Result<NoteModel>> CreateNote(Guid requestId, NoteModel model);
    Task<Result> UpdateNote(Guid requestId, Guid noteId, NoteModel model);
    Task<Result> DeleteNote(Guid requestId, Guid noteId);
    Task<Result> UpdateWorkerStartWorking(Guid requestId, Guid workerRequestId, DateTime startWorking);
    Task<Result<NoteModel>> CreateWorkerNote(Guid workerRequestId, NoteModel model);
    Task<Result> UpdateWorkerNote(Guid noteId, NoteModel model);
    Task<Result> DeleteWorkerNote(Guid noteId);
}
