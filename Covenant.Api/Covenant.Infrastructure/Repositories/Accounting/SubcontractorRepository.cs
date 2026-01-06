using Covenant.Common.Entities.Accounting.Subcontractor;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Models.Accounting.Subcontractor;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Repositories.Accounting;

public class SubcontractorRepository : BaseRepository<ReportSubcontractor>, ISubcontractorRepository
{
    private readonly CovenantContext _context;

    public SubcontractorRepository(CovenantContext context) : base(context) => _context = context;

    public Task<List<ReportSubcontractorModel>> GetReportsSubcontractorSummary(DateTime weekEnding) =>
        (from rs in _context.ReportSubcontractor.Where(s => s.WeekEnding.Date == weekEnding.Date)
         join wp in _context.WorkerProfile on rs.WorkerProfileId equals wp.Id
         join wpu in _context.User on wp.WorkerId equals wpu.Id
         orderby wp.FirstName
         select new ReportSubcontractorModel
         {
             Email = wpu.Email,
             FullName = wp.FirstName + " " + wp.MiddleName + " " + wp.LastName + " " + wp.SecondLastName,
             WeekEnding = rs.WeekEnding,
             Deductions = rs.DeductionOthers,
             TotalNet = rs.TotalNet,
             PublicHoliday = rs.PublicHolidayPay,
             Items = (from rsw in rs.WageDetails
                      join tst in _context.TimeSheetTotalPayroll on rsw.TimeSheetTotalId equals tst.Id
                      join ts in _context.TimeSheet on tst.TimeSheetId equals ts.Id
                      join wr in _context.WorkerRequest on ts.WorkerRequestId equals wr.Id
                      join r in _context.Request on wr.RequestId equals r.Id
                      join cp in _context.CompanyProfile
                          on new { Cc = r.CompanyId, Ca = r.AgencyId } equals new { Cc = cp.CompanyId, Ca = cp.AgencyId }
                      select new ReportSubcontractorItemModel
                      {
                          WorkerRate = rsw.WorkerRate,
                          Company = cp.BusinessName,
                          Regular = rsw.Regular,
                          OtherRegular = rsw.OtherRegular,
                          RegularHours = tst.RegularHours.TotalHours,
                          OtherRegularHours = tst.OtherRegularHours.TotalHours,
                          Overtime = rsw.Overtime,
                          OvertimeHours = tst.OvertimeHours.TotalHours,
                          Holiday = rsw.Holiday,
                          HolidayHours = tst.HolidayHours.TotalHours,
                          Missing = rsw.Missing,
                          MissingHours = ts.MissingHours.TotalHours,
                          MissingOvertime = rsw.MissingOvertime,
                          MissingOvertimeHours = ts.MissingHoursOvertime.TotalHours,
                          Others = ts.BonusOrOthers
                      }).ToList()
         }).ToListAsync();

    public async Task<PaginatedList<PayrollSubContractorListModel>> GetPayrollsSubcontractor(Guid agencyId, Pagination pagination)
    {
        var query = from ps in _context.ReportSubcontractor.Where(rs => rs.WorkerProfile.AgencyId == agencyId)
                    select new { ps.WeekEnding, ps.TotalNet };
        var data = query
            .GroupBy(a => a.WeekEnding.Date)
            .Select(a => new PayrollSubContractorListModel
            {
                TotalNet = a.Sum(t => t.TotalNet),
                WeekEnding = a.Key,
                NumberOfWorkers = a.Count()
            }).OrderByDescending(s => s.WeekEnding);
        return await data.ToPaginatedList(pagination);
    }

    public async Task<RegularWageWorker> GetSubcontractorRegularWages(ParamsToGetRegularWages p)
    {
        var queryable = from ps1 in _context.ReportSubcontractor.Where(s => s.WorkerProfileId == p.ProfileId && s.DateWorkEnd.Date >= p.Start && s.DateWorkEnd.Date <= p.End)
                        join wp1 in _context.WorkerProfile.Where(wp1W => wp1W.Id == p.ProfileId) on ps1.WorkerProfileId equals wp1.Id
                        group ps1 by ps1.WorkerProfileId
                        into result
                        select new
                        {
                            RegularWage = result.Sum(ps => ps.RegularWage)
                        };
        var data = queryable.Select(w => new RegularWageWorker
        {
            RegularWage = w.RegularWage,
            HolidayWasPaid = (from ps2 in _context.ReportSubcontractor.Where(s => s.WorkerProfileId == p.ProfileId)
                              join psh in _context.ReportSubcontractorPublicHolidays.Where(h => h.Holiday == p.Holiday) on ps2.Id equals psh.ReportSubcontractorId
                              select psh.Holiday).Any(),
            CustomPublicHolidayValue = (from wph in _context.WorkerProfileHoliday.Where(ph => ph.WorkerProfileId == p.ProfileId)
                                        join h in _context.Holiday.Where(hw => hw.Date.Date == p.Holiday.Date) on wph.HolidayId equals h.Id
                                        select wph.StatPaidWorker).FirstOrDefault(),
            IsEntitledToReceiveHolidayPay = (from wp in _context.WorkerProfile.Where(pr => pr.Id == p.ProfileId)
                                             join wr in _context.WorkerRequest on wp.WorkerId equals wr.WorkerId
                                             join isEntitledToReceiveHolidayPay in _context.TimeSheet.Where(s => p.RangeOfDaysWorkerMustWorkToReceiveHolidayPay.Contains(s.Date.Date)) on wr.Id equals isEntitledToReceiveHolidayPay.WorkerRequestId
                                             select isEntitledToReceiveHolidayPay.Date).Any()
        });
        return await data.SingleOrDefaultAsync();
    }
}