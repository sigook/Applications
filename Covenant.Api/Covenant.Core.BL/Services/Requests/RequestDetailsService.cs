using Covenant.Common.Entities;
using Covenant.Common.Entities.Request;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
using Covenant.Common.Models.Request.Agency;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Requests;
using Covenant.Core.BL.Interfaces.Requests;
using Covenant.Core.BL.Interfaces.Shared;

namespace Covenant.Core.BL.Services.Requests;

public class RequestDetailsService(
    IRequestRepository requestRepository,
    IWorkerRequestRepository workerRequestRepository,
    IShiftRepository shiftRepository,
    ISkillService skillService,
    ICurrentUserService currentUserService) : IRequestDetailsService
{
    private const string RequestNotFound = "Request not found";
    private const string NoteNotFound = "Note not found";
    private const string ContactAlreadyAdded = "The contact person is already in the request";
    private const string ContactNotFound = "Contact person not found";

    public async Task<Result<AgencyRequestDetailModel>> UpdateShift(Guid requestId, ShiftModel model)
    {
        var entity = await requestRepository.GetRequest(r => r.Id == requestId);
        if (entity is null) return Result.Fail<AgencyRequestDetailModel>(RequestNotFound);
        var newShift = model.ToShift();
        entity.OnNewShift += async (sender, e) => await shiftRepository.Create(newShift);
        var result = entity.UpdateShift(newShift);
        if (!result) return Result.Fail<AgencyRequestDetailModel>(result.Errors);
        requestRepository.Update(entity);
        await requestRepository.SaveChangesAsync();
        return Result.Ok(new AgencyRequestDetailModel { Id = entity.Id, DisplayShift = newShift.DisplayShift });
    }

    public async Task<Result> AddReportTo(Guid requestId, Guid contactPersonId)
    {
        var existing = await requestRepository.GetReportTo(requestId, contactPersonId);
        if (existing is not null) return Result.Fail(ContactAlreadyAdded);
        await requestRepository.Create(new RequestReportTo(requestId, contactPersonId));
        await requestRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> DeleteReportTo(Guid requestId, Guid contactPersonId)
    {
        var entity = await requestRepository.GetReportTo(requestId, contactPersonId);
        if (entity is null) return Result.Fail(ContactNotFound);
        requestRepository.Delete(entity);
        await requestRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> AddRequestedBy(Guid requestId, Guid contactPersonId)
    {
        var existing = await requestRepository.GetRequestedBy(requestId, contactPersonId);
        if (existing is not null) return Result.Fail(ContactAlreadyAdded);
        await requestRepository.Create(new RequestRequestedBy(requestId, contactPersonId));
        await requestRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> DeleteRequestedBy(Guid requestId, Guid contactPersonId)
    {
        var entity = await requestRepository.GetRequestedBy(requestId, contactPersonId);
        if (entity is null) return Result.Fail(ContactNotFound);
        requestRepository.Delete(entity);
        await requestRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<SkillModel>> AddSkill(Guid requestId, string skill)
    {
        var canonicalSkill = (await skillService.Canonicalize([skill])).FirstOrDefault();
        var result = RequestSkill.Create(requestId, canonicalSkill);
        if (!result) return Result.Fail<SkillModel>(result.Errors);
        await requestRepository.Create(result.Value);
        await requestRepository.SaveChangesAsync();
        return Result.Ok(new SkillModel(canonicalSkill) { Id = result.Value.Id });
    }

    public async Task<Result> DeleteSkill(Guid requestId, Guid skillId)
    {
        var entity = await requestRepository.GetSkill(requestId, skillId);
        if (entity is null) return Result.Fail("Skill not found");
        requestRepository.Delete(entity);
        await requestRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<NoteModel>> CreateNote(Guid requestId, NoteModel model)
    {
        var note = CovenantNote.Create(model.Note, model.Color, currentUserService.GetNickname());
        if (!note) return Result.Fail<NoteModel>(note.Errors);
        var entity = new RequestNote(requestId, note.Value);
        await requestRepository.Create(entity);
        await requestRepository.SaveChangesAsync();
        return Result.Ok(new NoteModel
        {
            Id = entity.NoteId,
            CreatedBy = entity.Note.CreatedBy,
            CreatedAt = entity.Note.CreatedAt,
            Color = entity.Note.Color
        });
    }

    public async Task<Result> UpdateNote(Guid requestId, Guid noteId, NoteModel model)
    {
        var nickname = currentUserService.GetNickname();
        var note = CovenantNote.Create(model.Note, model.Color, nickname);
        if (!note) return note;
        var entity = await requestRepository.GetNote(requestId, noteId);
        if (entity is null) return Result.Fail(NoteNotFound);
        entity.Update(note.Value, nickname);
        requestRepository.Update(entity);
        await requestRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> DeleteNote(Guid requestId, Guid noteId)
    {
        var entity = await requestRepository.GetNote(requestId, noteId);
        if (entity is null) return Result.Fail(NoteNotFound);
        entity.Delete(currentUserService.GetNickname());
        requestRepository.Update(entity);
        await requestRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> UpdateWorkerStartWorking(Guid requestId, Guid workerRequestId, DateTime startWorking)
    {
        var entity = await workerRequestRepository.GetWorkerRequest(workerRequestId);
        if (entity is null || entity.RequestId != requestId) return Result.Fail("Worker not found in the request");
        var result = entity.UpdateStartWorking(startWorking);
        if (!result) return result;
        workerRequestRepository.Update(entity);
        await workerRequestRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<NoteModel>> CreateWorkerNote(Guid workerRequestId, NoteModel model)
    {
        var note = CovenantNote.Create(model.Note, model.Color, currentUserService.GetNickname());
        if (!note) return Result.Fail<NoteModel>(note.Errors);
        var entity = new WorkerRequestNote(workerRequestId, note.Value);
        await workerRequestRepository.Create(entity);
        await workerRequestRepository.SaveChangesAsync();
        return Result.Ok(new NoteModel
        {
            Id = entity.NoteId,
            CreatedBy = entity.Note.CreatedBy,
            CreatedAt = entity.Note.CreatedAt,
            Color = entity.Note.Color
        });
    }

    public async Task<Result> UpdateWorkerNote(Guid noteId, NoteModel model)
    {
        var nickname = currentUserService.GetNickname();
        var note = CovenantNote.Create(model.Note, model.Color, nickname);
        if (!note) return note;
        var entity = await workerRequestRepository.GetNote(noteId);
        if (entity is null) return Result.Fail(NoteNotFound);
        entity.Update(note.Value, nickname);
        workerRequestRepository.Update(entity);
        await workerRequestRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> DeleteWorkerNote(Guid noteId)
    {
        var entity = await workerRequestRepository.GetNote(noteId);
        if (entity is null) return Result.Fail(NoteNotFound);
        entity.Delete(currentUserService.GetNickname());
        workerRequestRepository.Update(entity);
        await workerRequestRepository.SaveChangesAsync();
        return Result.Ok();
    }
}
