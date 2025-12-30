using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Covenant.Infrastructure.Repositories.Request
{
    public class WorkerRequestRepository : IWorkerRequestRepository
    {
        private readonly CovenantContext _context;

        public WorkerRequestRepository(CovenantContext context) => _context = context;

        public Task<Common.Entities.Request.WorkerRequest> GetWorkerRequest(Guid id) => GetWorkerRequest(w => w.Id == id);

        public Task<Common.Entities.Request.WorkerRequest> GetWorkerRequest(Guid workerId, Guid requestId) => GetWorkerRequest(c => c.WorkerId == workerId && c.RequestId == requestId);

        private Task<Common.Entities.Request.WorkerRequest> GetWorkerRequest(Expression<Func<Common.Entities.Request.WorkerRequest, bool>> condition) =>
            _context.WorkerRequest.Where(condition)
                .Include(r => r.TimeSheets)
                .Include(r => r.Request)
                .ThenInclude(r => r.JobLocation)
                .ThenInclude(jl => jl.City)
                .ThenInclude(c => c.Province)
                .ThenInclude(p => p.Country)
                .FirstOrDefaultAsync();

        public Task<Common.Entities.Request.WorkerRequest> GetWorkerRequestByWorkerProfileId(Guid workerProfileId, Guid requestId) =>
            (from workerProfile in _context.WorkerProfile.Where(wW => wW.Id == workerProfileId)
             join workerRequest in _context.WorkerRequest.Where(rW => rW.RequestId == requestId) on workerProfile.WorkerId equals workerRequest.WorkerId
             select workerRequest).Include(i => i.TimeSheets).SingleOrDefaultAsync();

        public async Task<IEnumerable<Common.Entities.Request.WorkerRequest>> GetWorkerRequestsByWorkerId(Guid workerId)
        {
            var workerRequest = await _context.WorkerRequest.Include(wr => wr.Request).ThenInclude(r => r.Shift)
                .Where(wp => wp.WorkerId == workerId && wp.WorkerRequestStatus == WorkerRequestStatus.Booked)
                .ToListAsync();
            return workerRequest;
        }

        public Task<bool> WorkerRequestExists(Guid workerId, Guid requestId) =>
            _context.WorkerRequest.AnyAsync(a => a.WorkerId == workerId && a.RequestId == requestId);

        public async Task<bool> CanClockIn(Expression<Func<WorkerProfile, bool>> condition, DateTime now) =>
            await (from wp in _context.WorkerProfile.Where(condition)
                   join wr in _context.WorkerRequest.Where(w => w.WorkerRequestStatus == WorkerRequestStatus.Booked && w.StartWorking <= now) on wp.WorkerId equals wr.WorkerId
                   join r in _context.Request.Where(wR => wR.Status == RequestStatus.Requested || wR.Status == RequestStatus.InProcess) on wr.RequestId equals r.Id
                   select wr.Id).AnyAsync();

        public async Task<WorkerRequestInfoModel> GetWorkerRequestInfo(Guid workerId, Guid requestId, DateTime now)
        {
            var workerRequest = _context.WorkerRequest
                .Include(wr => wr.Request)
                .ThenInclude(r => r.JobLocation)
                .Where(wr => wr.RequestId == requestId && wr.StartWorking <= now);
            var query = from wp in _context.WorkerProfile.Where(wp => wp.WorkerId == workerId)
                        join wr in workerRequest on wp.WorkerId equals wr.WorkerId
                        orderby wr.StartWorking
                        select new WorkerRequestInfoModel
                        {
                            WorkerFullName = wp.FirstName + " " + wp.MiddleName + " " + wp.LastName + " " + wp.SecondLastName,
                            WorkerRequestId = wr.Id,
                            Shift = wr.Request.Shift,
                            StartWorking = wr.StartWorking,
                            Latitude = wr.Request.JobLocation.Latitude,
                            Longitude = wr.Request.JobLocation.Longitude,
                            CountryCode = wr.Request.JobLocation.City.Province.Country.Code
                        };
            return await query.FirstOrDefaultAsync();
        }

        public async Task CreateNote(WorkerRequestNote entity) => await _context.WorkerRequestNote.AddAsync(entity);

        public Task<PaginatedList<NoteModel>> GetNotes(Guid workerRequestId, Pagination pagination) =>
            _context.WorkerRequestNote.Where(w => w.WorkerRequestId == workerRequestId && !w.Note.IsDeleted)
                .Select(WorkerRequestExtensionsMapping.SelectNote).OrderByDescending(c => c.CreatedAt).ToPaginatedList(pagination);

        public Task<NoteModel> GetNoteDetail(Guid id) => _context.WorkerRequestNote.Where(w => w.NoteId == id).Select(WorkerRequestExtensionsMapping.SelectNote).SingleOrDefaultAsync();

        public Task<WorkerRequestNote> GetNote(Guid id) => _context.WorkerRequestNote.Where(c => c.NoteId == id).Include(c => c.Note).SingleOrDefaultAsync();

        public Task Update(WorkerRequestNote entity) => Task.FromResult(_context.WorkerRequestNote.Update(entity));

        public Task UpdateWorkerRequest(Common.Entities.Request.WorkerRequest entity) => Task.FromResult(_context.WorkerRequest.Update(entity));

        public Task SaveChangesAsync() => _context.SaveChangesAsync();
    }
}