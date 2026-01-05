using Covenant.Common.Configuration;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Models;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Covenant.Infrastructure.Repositories.Worker
{
    public class WorkerCommentsRepository : IWorkerCommentsRepository
    {
        private readonly CovenantContext _context;
        private readonly FilesConfiguration filesConfiguration;

        public WorkerCommentsRepository(CovenantContext context, IOptions<FilesConfiguration> options)
        {
            _context = context;
            filesConfiguration = options.Value;
        }

        public Task CreateComment(WorkerComment comment)
        {
            _context.WorkerComment.Add(comment);
            return _context.SaveChangesAsync();
        }

        public async Task<PaginatedList<WorkerCommentModel>> GetComments(Guid workerId, Pagination pagination)
        {
            var query = (from wc in _context.WorkerComment.Where(c => c.WorkerId == workerId)
                         select new WorkerCommentModel
                         {
                             Id = wc.Id,
                             Comment = wc.Comment,
                             Rate = wc.Rate,
                             NumberId = wc.NumberId,
                             CreatedAt = wc.CreatedAt,
                             Logo = wc.AgencyId != null
                                 ? (from a in _context.Agencies.Where(c => c.Id == wc.AgencyId)
                                    join cf in _context.CovenantFile on a.LogoId equals cf.Id into tmp
                                    from cfl in tmp.DefaultIfEmpty()
                                    select $"{filesConfiguration.FilesPath}{cfl.FileName}").FirstOrDefault()
                                 : (from cp in _context.CompanyProfile.Where(c => c.CompanyId == wc.CompanyId)
                                    join cf in _context.CovenantFile on cp.LogoId equals cf.Id into tmp1
                                    from cfl in tmp1.DefaultIfEmpty()
                                    select $"{filesConfiguration.FilesPath}{cfl.FileName}").FirstOrDefault()
                         }).OrderByDescending(c => c.CreatedAt);

            return await query.ToPaginatedList(pagination);
        }

        public Task<WorkerComment> GetComment(Guid workerId, Guid id) => _context.WorkerComment.FirstOrDefaultAsync(c => c.WorkerId == workerId && c.Id == id);

        public Task<WorkerCommentModel> GetCommentModel(Guid workerId, Guid id)
        {
            var query = from wc in _context.WorkerComment.Where(c => c.Id == id && c.WorkerId == workerId)
                        select new WorkerCommentModel
                        {
                            Id = wc.Id,
                            Comment = wc.Comment,
                            Rate = wc.Rate,
                            NumberId = wc.NumberId,
                            CreatedAt = wc.CreatedAt,
                            Logo = wc.AgencyId != null
                                ? (from a in _context.Agencies.Where(c => c.Id == wc.AgencyId)
                                   join cf in _context.CovenantFile on a.LogoId equals cf.Id into tmp
                                   from cfl in tmp.DefaultIfEmpty()
                                   select $"{filesConfiguration.FilesPath}{cfl.FileName}").FirstOrDefault()
                                : (from cp in _context.CompanyProfile.Where(c => c.CompanyId == wc.CompanyId)
                                   join cf in _context.CovenantFile on cp.LogoId equals cf.Id into tmp1
                                   from cfl in tmp1.DefaultIfEmpty()
                                   select $"{filesConfiguration.FilesPath}{cfl.FileName}").FirstOrDefault()
                        };

            return query.SingleOrDefaultAsync();
        }

        public Task UpdateComment(WorkerComment comment)
        {
            _context.WorkerComment.Update(comment);
            return _context.SaveChangesAsync();
        }
    }
}