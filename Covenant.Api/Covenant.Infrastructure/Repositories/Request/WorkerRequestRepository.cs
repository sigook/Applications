using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Covenant.Infrastructure.Repositories.Request;

public class WorkerRequestRepository : IWorkerRequestRepository
{
    private readonly CovenantContext _context;

    public WorkerRequestRepository(CovenantContext context) => _context = context;

    public Task<WorkerRequest> GetWorkerRequest(Guid id) => GetWorkerRequest(w => w.Id == id);

    public Task<WorkerRequest> GetWorkerRequest(Guid workerId, Guid requestId) => GetWorkerRequest(c => c.WorkerProfile.WorkerId == workerId && c.RequestId == requestId);

    private Task<WorkerRequest> GetWorkerRequest(Expression<Func<WorkerRequest, bool>> condition) =>
        _context.WorkerRequests.Where(condition)
            .Include(r => r.TimeSheets)
            .Include(r => r.Request)
            .ThenInclude(r => r.JobLocation)
            .ThenInclude(jl => jl.City)
            .ThenInclude(c => c.Province)
            .ThenInclude(p => p.Country)
            .FirstOrDefaultAsync();

    public Task<WorkerRequest> GetWorkerRequestByWorkerProfileId(Guid workerProfileId, Guid requestId) =>
        _context.WorkerRequests.Where(wr => wr.WorkerProfileId == workerProfileId && wr.RequestId == requestId)
            .Include(i => i.TimeSheets)
            .Include(i => i.Request)
            .ThenInclude(r => r.JobLocation)
            .ThenInclude(jl => jl.City)
            .ThenInclude(c => c.Province)
            .ThenInclude(p => p.Country).SingleOrDefaultAsync();

    public async Task<IEnumerable<WorkerRequest>> GetWorkerRequestsByWorkerProfileId(Guid workerProfileId)
    {
        var workerRequest = await _context.WorkerRequests.Include(wr => wr.Request).ThenInclude(r => r.Shift)
            .Where(wr => wr.WorkerProfileId == workerProfileId && wr.WorkerRequestStatus == WorkerRequestStatus.Booked)
            .ToListAsync();
        return workerRequest;
    }

    public Task<bool> WorkerRequestExists(Guid workerProfileId, Guid requestId) =>
        _context.WorkerRequests.AnyAsync(a => a.WorkerProfileId == workerProfileId && a.RequestId == requestId);

    public async Task<bool> CanClockIn(Expression<Func<WorkerProfile, bool>> condition, DateTime now) =>
        await (from wp in _context.WorkerProfiles.Where(condition)
               join wr in _context.WorkerRequests.Where(w => w.WorkerRequestStatus == WorkerRequestStatus.Booked
                                                            && w.StartWorking <= now
                                                            && w.Request.Status == RequestStatus.Open) on wp.Id equals wr.WorkerProfileId
               select wr.Id).AnyAsync();

    public async Task<WorkerRequestInfoModel> GetWorkerRequestInfo(Guid workerId, Guid requestId, DateTime now)
    {
        var workerRequest = _context.WorkerRequests
            .Include(wr => wr.Request)
            .ThenInclude(r => r.JobLocation)
            .Where(wr => wr.RequestId == requestId && wr.StartWorking <= now);
        var query = from wr in workerRequest.Where(wr => wr.WorkerProfile.WorkerId == workerId)
                    orderby wr.StartWorking
                    select new WorkerRequestInfoModel
                    {
                        WorkerFullName = wr.WorkerProfile.FirstName + " " + wr.WorkerProfile.MiddleName + " " + wr.WorkerProfile.LastName + " " + wr.WorkerProfile.SecondLastName,
                        WorkerRequestId = wr.Id,
                        Shift = wr.Request.Shift,
                        StartWorking = wr.StartWorking,
                        Latitude = wr.Request.JobLocation.Latitude,
                        Longitude = wr.Request.JobLocation.Longitude,
                        CountryCode = wr.Request.JobLocation.City.Province.Country.Code
                    };
        return await query.FirstOrDefaultAsync();
    }

    public async Task CreateNote(WorkerRequestNote entity) => await _context.WorkerRequestNotes.AddAsync(entity);

    public Task<PaginatedList<NoteModel>> GetNotes(Guid workerRequestId, Pagination pagination) =>
        _context.WorkerRequestNotes.Where(w => w.WorkerRequestId == workerRequestId && !w.Note.IsDeleted)
            .Select(WorkerRequestExtensionsMapping.SelectNote).OrderByDescending(c => c.CreatedAt).ToPaginatedList(pagination);

    public Task<NoteModel> GetNoteDetail(Guid id) => _context.WorkerRequestNotes.Where(w => w.NoteId == id).Select(WorkerRequestExtensionsMapping.SelectNote).SingleOrDefaultAsync();

    public Task<WorkerRequestNote> GetNote(Guid id) => _context.WorkerRequestNotes.Where(c => c.NoteId == id).Include(c => c.Note).SingleOrDefaultAsync();

    public Task Update(WorkerRequestNote entity) => Task.FromResult(_context.WorkerRequestNotes.Update(entity));

    public Task UpdateWorkerRequest(WorkerRequest entity) => Task.FromResult(_context.WorkerRequests.Update(entity));

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}