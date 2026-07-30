using Covenant.Common.Entities.Accounting.Subcontractor;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Models.Accounting.Subcontractor;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Repositories.Accounting;

public class SubcontractorRepository : BaseRepository<ReportSubcontractor>, ISubcontractorRepository
{
    private readonly CovenantContext _context;

    public SubcontractorRepository(CovenantContext context) : base(context) => _context = context;

    public Task<List<ReportSubcontractorModel>> GetReportsSubcontractorSummary(DateTime weekEnding) =>
        (from rs in _context.ReportSubcontractor.Where(s => s.WeekEnding.Date == weekEnding.Date)
         orderby rs.WorkerProfile.FirstName
         select new ReportSubcontractorModel
         {
             Email = rs.WorkerProfile.Worker.Email,
             FullName = rs.WorkerProfile.FirstName + " " + rs.WorkerProfile.MiddleName + " " + rs.WorkerProfile.LastName + " " + rs.WorkerProfile.SecondLastName,
             WeekEnding = rs.WeekEnding,
             Deductions = rs.DeductionOthers,
             TotalNet = rs.TotalNet,
             PublicHoliday = rs.PublicHolidayPay,
             Items = (from rsw in rs.WageDetails
                      select new ReportSubcontractorItemModel
                      {
                          WorkerRate = rsw.WorkerRate,
                          Company = rsw.TimeSheetTotal.TimeSheet.WorkerRequest.Request.CompanyProfile.FullName,
                          Regular = rsw.Regular,
                          OtherRegular = rsw.OtherRegular,
                          RegularHours = rsw.TimeSheetTotal.RegularHours.TotalHours,
                          OtherRegularHours = rsw.TimeSheetTotal.OtherRegularHours.TotalHours,
                          Overtime = rsw.Overtime,
                          OvertimeHours = rsw.TimeSheetTotal.OvertimeHours.TotalHours,
                          Holiday = rsw.Holiday,
                          HolidayHours = rsw.TimeSheetTotal.HolidayHours.TotalHours,
                          Missing = rsw.Missing,
                          MissingHours = rsw.TimeSheetTotal.TimeSheet.MissingHours.TotalHours,
                          MissingOvertime = rsw.MissingOvertime,
                          MissingOvertimeHours = rsw.TimeSheetTotal.TimeSheet.MissingHoursOvertime.TotalHours,
                          Others = rsw.TimeSheetTotal.TimeSheet.BonusOrOthers
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

    public async Task<RegularWageWorker> GetSubcontractorRegularWages(Guid workerProfileId, DateTime holiday, DateTime start, DateTime end, IEnumerable<DateTime> qualifyingDays)
    {
        var queryable = from ps1 in _context.ReportSubcontractor.Where(s => s.WorkerProfileId == workerProfileId && s.DateWorkEnd.Date >= start && s.DateWorkEnd.Date <= end)
                        group ps1 by ps1.WorkerProfileId
                        into result
                        select new
                        {
                            RegularWage = result.Sum(ps => ps.RegularWage)
                        };
        var data = queryable.Select(w => new RegularWageWorker
        {
            RegularWage = w.RegularWage,
            HolidayWasPaid = _context.ReportSubcontractorPublicHolidays
                .Any(psh => psh.Holiday == holiday && psh.ReportSubcontractor.WorkerProfileId == workerProfileId),
            CustomPublicHolidayValue = _context.WorkerProfileHoliday
                .Where(wph => wph.WorkerProfileId == workerProfileId && wph.Holiday.Date.Date == holiday.Date)
                .Select(wph => wph.StatPaidWorker)
                .FirstOrDefault(),
            IsEntitledToReceiveHolidayPay = _context.TimeSheet
                .Any(ts => ts.WorkerRequest.WorkerProfileId == workerProfileId
                    && qualifyingDays.Contains(ts.Date.Date))
        });
        return await data.SingleOrDefaultAsync();
    }
}