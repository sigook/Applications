using Covenant.Common.Configuration;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Covenant.Infrastructure.Repositories.Request;

public class TimeSheetRepository : ITimeSheetRepository
{
    private readonly CovenantContext _context;
    private readonly Rates rates;
    private readonly IRequestRepository requestRepository;
    private readonly TimeLimits timeLimits;

    public TimeSheetRepository(
        CovenantContext context,
        Rates rates,
        IRequestRepository requestRepository,
        TimeLimits timeLimits)
    {
        _context = context;
        this.rates = rates;
        this.requestRepository = requestRepository;
        this.timeLimits = timeLimits;
    }

    public async Task Create<T>(T entity) where T : class => await _context.Set<T>().AddAsync(entity);

    public void Delete<T>(T entity) where T : class => _context.Set<T>().Remove(entity);

    public Task SaveChangesAsync() => _context.SaveChangesAsync();

    public Task<TimeSheet> GetTimeSheet(Guid id) => _context.TimeSheet.SingleOrDefaultAsync(s => s.Id == id);

    public async Task<IEnumerable<TimeSheet>> GetTimeSheets(Guid workerId, Guid requestId, Expression<Func<Common.Entities.Request.Request, bool>> requestCondition)
    {
        var query = from wr in _context.WorkerRequest.Where(c => c.WorkerId == workerId && c.RequestId == requestId)
                    join r in _context.Request.Where(requestCondition) on wr.RequestId equals r.Id
                    join ts in _context.TimeSheet on wr.Id equals ts.WorkerRequestId
                    select ts;
        return await query.ToListAsync();
    }

    public async Task<IEnumerable<TimeSheetListModel>> GetTimeSheetsListModel(Guid workerId, Guid requestId, DateTime? startDate, DateTime? endDate)
    {
        var timeSheet = _context.TimeSheet.AsQueryable();
        if (startDate.HasValue)
        {
            timeSheet = timeSheet.Where(ts => ts.Date >= startDate.Value);
        }
        if (endDate.HasValue)
        {
            timeSheet = timeSheet.Where(ts => ts.Date <= endDate.Value);
        }
        var query = from wr in _context.WorkerRequest.Where(c => c.WorkerId == workerId && c.RequestId == requestId)
                    join ts in timeSheet on wr.Id equals ts.WorkerRequestId
                    join tst in _context.TimeSheetTotal on ts.Id equals tst.TimeSheetId into tmp
                    from tst in tmp.DefaultIfEmpty()
                    join tstP in _context.TimeSheetTotalPayroll on ts.Id equals tstP.TimeSheetId into tmp2
                    from tstP in tmp2.DefaultIfEmpty()
                    orderby ts.Date
                    select new TimeSheetListModel
                    {
                        Id = ts.Id,
                        Day = ts.Date,
                        ClockIn = ts.ClockIn,
                        ClockOut = ts.ClockOut,
                        ClockInRounded = ts.ClockInRounded,
                        ClockOutRounded = ts.ClockOutRounded,
                        TimeIn = ts.TimeIn,
                        TimeOut = ts.TimeOut,
                        TimeInApproved = ts.TimeInApproved,
                        TimeOutApproved = ts.TimeOutApproved,
                        MissingHours = ts.MissingHours,
                        MissingHoursOvertime = ts.MissingHoursOvertime,
                        MissingRateWorker = ts.MissingRateWorker,
                        MissingRateAgency = ts.MissingRateAgency,
                        DeductionsOthers = ts.DeductionsOthers,
                        DeductionsOthersDescription = ts.DeductionsOthersDescription,
                        Comment = ts.Comment,
                        CanUpdate = tst == null && tstP == null,
                        WasApproved = ts.TimeInApproved != null && ts.TimeOutApproved != null,
                        Week = PostgresFunctions.get_week_start_sunday(ts.Date),
                        BonusOrOthers = ts.BonusOrOthers,
                        BonusOrOthersDescription = ts.BonusOrOthersDescription,
                        Reimbursements = ts.Reimbursements,
                        ReimbursementsDescription = ts.ReimbursementsDescription,
                        TotalHours = ts.TotalHours,
                        TotalHoursApproved = ts.TotalHoursApproved
                    };
        var result = await query.ToListAsync();
        return result;
    }

    public async Task<PaginatedList<TimeSheetHistoryModel>> GetTimeSheetHistory(Guid workerProfileId, Pagination pagination)
    {
        var query = from th in _context.TimesheetHistories.Where(th => th.WorkerProfileId == workerProfileId)
                    select new TimeSheetHistoryModel
                    {
                        RowNumber = th.RowNumber,
                        NumberId = th.NumberId,
                        BusinessName = th.CompanyName,
                        JobTitle = th.JobTitle,
                        Date = th.Date,
                        IsHoliday = th.IsHoliday,
                        RegularHours = !th.RegularHours.HasValue ? 0 : th.RegularHours.Value.TotalHours,
                        HolidayHours = !th.HolidayHours.HasValue ? 0 : th.HolidayHours.Value.TotalHours,
                        OvertimeHours = !th.OvertimeHours.HasValue ? 0 : th.OvertimeHours.Value.TotalHours,
                        MissingHours = !th.MissingHours.HasValue ? 0 : th.MissingHours.Value.TotalHours,
                        MissingHoursOvertime = !th.MissingHoursOvertime.HasValue ? 0 : th.MissingHoursOvertime.Value.TotalHours
                    };
        var result = await query.ToPaginatedList(pagination);
        return result;
    }

    public async Task<TimesheetHistoryAccumulated> GetTimesheetHistoryAccumulated(Guid workerProfileId, int rowNumber)
    {
        var timesheets = await _context.TimesheetHistories
            .Where(th => th.WorkerProfileId == workerProfileId && th.RowNumber <= rowNumber)
            .ToListAsync();
        var result = new TimesheetHistoryAccumulated
        {
            RegularHours = timesheets.Sum(th => th.RegularHours?.TotalHours ?? 0d),
            HolidayHours = timesheets.Sum(th => th.HolidayHours?.TotalHours ?? 0d),
            OvertimeHours = timesheets.Sum(th => th.OvertimeHours?.TotalHours ?? 0d),
            MissingHours = timesheets.Sum(th => th.MissingHours?.TotalHours ?? 0d),
            MissingHoursOvertime = timesheets.Sum(th => th.MissingHoursOvertime?.TotalHours ?? 0d)
        };
        return result;
    }

    public async Task<PaginatedList<TimeSheetListModel>> GetTimeSheetsForWorker(Guid workerId, Guid requestId, Pagination pagination)
    {
        var query = (from workerRequest in _context.WorkerRequest
                     join timeSheet in _context.TimeSheet on workerRequest.Id equals timeSheet.WorkerRequestId
                     where workerRequest.WorkerId == workerId && workerRequest.RequestId == requestId
                     select new TimeSheetListModel
                     {
                         Id = timeSheet.Id,
                         Day = timeSheet.Date,
                         ClockIn = timeSheet.ClockIn,
                         ClockOut = timeSheet.ClockOut,
                         ClockInRounded = timeSheet.ClockInRounded,
                         ClockOutRounded = timeSheet.ClockOutRounded,
                         TimeIn = timeSheet.TimeIn,
                         TimeOut = timeSheet.TimeOut,
                         TimeInApproved = timeSheet.TimeInApproved,
                         TimeOutApproved = timeSheet.TimeOutApproved,
                         Comment = timeSheet.Comment,
                         TotalHours = timeSheet.TotalHours,
                         TotalHoursApproved = timeSheet.TotalHoursApproved,
                     }).OrderByDescending(c => c.Day);
        return await query.ToPaginatedList(pagination);
    }

    public async Task<List<TimeSheetApprovedBillingModel>> GetTimeSheetForCreatingInvoice(IEnumerable<Guid> agencyIds, CreateInvoiceModel model)
    {
        var timeSheet = _context.TimeSheet
            .Include(ts => ts.WorkerRequest)
            .ThenInclude(ts => ts.Request)
            .ThenInclude(ts => ts.Agency)
            .Where(ts => agencyIds.Contains(ts.WorkerRequest.Request.AgencyId) && ts.WorkerRequest.Request.CompanyId == model.CompanyId)
            .Where(ts => ts.TimeInApproved != null && ts.TimeOutApproved != null && ts.TimeSheetTotal == null);
        if (model.ProvinceId.HasValue)
        {
            timeSheet = timeSheet.Where(ts => ts.WorkerRequest.Request.JobLocation.City.Province.Id == model.ProvinceId);
        }
        else
        {
            timeSheet = timeSheet.Where(ts => ts.WorkerRequest.Request.JobLocation.City.Province.ProvinceTax.Tax1 == 0);
        }
        if (model.From.HasValue && model.To.HasValue)
        {
            timeSheet = timeSheet.Where(ts => ts.Date.Date >= model.From.Value && ts.Date.Date <= model.To.Value);
        }
        var query = from ts in timeSheet
                    join cp in _context.CompanyProfile on ts.WorkerRequest.Request.CompanyId equals cp.CompanyId
                    select new TimeSheetApprovedBillingModel
                    {
                        PaidHolidays = cp.PaidHolidays,
                        OvertimeStartsAfter = ts.WorkerRequest.Request.JobLocation.City.Province.ProvinceSetting != null &&
                            ts.WorkerRequest.Request.JobLocation.City.Province.ProvinceSetting.OvertimeStartsAfter.HasValue
                            ? ts.WorkerRequest.Request.JobLocation.City.Province.ProvinceSetting.OvertimeStartsAfter.Value
                            : cp.OvertimeStartsAfter,
                        RequestId = ts.WorkerRequest.RequestId,
                        JobTitle = string.IsNullOrWhiteSpace(ts.WorkerRequest.Request.BillingTitle) ? ts.WorkerRequest.Request.JobTitle : ts.WorkerRequest.Request.BillingTitle,
                        AgencyRate = ts.WorkerRequest.Request.AgencyRate.Value,
                        BreakIsPaid = ts.WorkerRequest.Request.BreakIsPaid,
                        DurationBreak = ts.WorkerRequest.Request.DurationBreak,
                        HolidayIsPaid = ts.WorkerRequest.Request.HolidayIsPaid,
                        WorkerId = ts.WorkerRequest.Worker.Id,
                        TimeSheetId = ts.Id,
                        Week = PostgresFunctions.get_week_start_sunday(ts.Date),
                        Date = ts.Date,
                        TimeInApproved = ts.TimeInApproved,
                        TimeOutApproved = ts.TimeOutApproved,
                        MissingHours = ts.MissingHours,
                        MissingHoursOvertime = ts.MissingHoursOvertime,
                        MissingRateAgency = ts.MissingRateAgency,
                        IsHoliday = ts.IsHoliday,
                        MaxHoursWeek = timeLimits.MaxHoursWeek,
                        OverTime = rates.OverTime,
                        NightShift = rates.NightShift,
                        Holiday = rates.Holiday
                    };
        var result = await query.ToListAsync();
        return result;
    }

    public Task<List<TimeSheetApprovedPayrollModel>> GetTimeSheetForCreatingReportsSubcontractor(IEnumerable<Guid> agencyIds, Guid companyId) =>
        GetTimeSheet(r => agencyIds.Contains(r.AgencyId) && r.CompanyId == companyId, wp => wp.IsSubcontractor);

    public Task<List<TimeSheetApprovedPayrollModel>> GetTimeSheetForCreatingPayStubs(IEnumerable<Guid> agencyIds, Guid workerId) =>
        GetTimeSheet(r => agencyIds.Contains(r.AgencyId), wp => wp.WorkerId == workerId && !wp.IsSubcontractor);

    private async Task<List<TimeSheetApprovedPayrollModel>> GetTimeSheet(
        Expression<Func<Common.Entities.Request.Request, bool>> conditionRequest,
        Expression<Func<WorkerProfile, bool>> conditionWorker)
    {
        var requests = _context.Request.Where(conditionRequest).Where(r => !r.WorkerSalary.HasValue);
        var workerProfiles = _context.WorkerProfile.Where(conditionWorker);
        var timeSheets = _context.TimeSheet.Where(c => c.TimeInApproved != null && c.TimeOutApproved != null && !_context.TimeSheetTotalPayroll.Any(ts => ts.TimeSheetId == c.Id));
        var query = from r in requests
                    join cpj in _context.CompanyProfileJobPositionRate on r.JobPositionRateId equals cpj.Id
                    join jp in _context.JobPosition on cpj.JobPositionId equals jp.Id into tmp1
                    from jp in tmp1.DefaultIfEmpty()
                    join wr in _context.WorkerRequest on r.Id equals wr.RequestId
                    join cp in _context.CompanyProfile on new { r.CompanyId, r.AgencyId } equals new { cp.CompanyId, cp.AgencyId }
                    join wp in workerProfiles on new { wr.WorkerId, r.AgencyId } equals new { wp.WorkerId, wp.AgencyId }
                    join ts in timeSheets on wr.Id equals ts.WorkerRequestId
                    select new TimeSheetApprovedPayrollModel(
                        r.JobLocation.City.Province.ProvinceSetting != null && r.JobLocation.City.Province.ProvinceSetting.OvertimeStartsAfter.HasValue
                            ? r.JobLocation.City.Province.ProvinceSetting.OvertimeStartsAfter.Value
                            : cp.OvertimeStartsAfter,
                        r.Id,
                        r.WorkerRate.Value,
                        r.BreakIsPaid,
                        r.DurationBreak,
                        r.HolidayIsPaid,
                        wp.WorkerId,
                        wp.Id,
                        ts.Id,
                        PostgresFunctions.get_week_start_sunday(ts.Date),
                        ts.Date,
                        ts.TimeInApproved,
                        ts.TimeOutApproved,
                        ts.MissingHours,
                        ts.MissingHoursOvertime,
                        ts.MissingRateWorker,
                        ts.IsHoliday,
                        ts.BonusOrOthers,
                        ts.BonusOrOthersDescription,
                        ts.DeductionsOthers,
                        ts.DeductionsOthersDescription,
                        ts.Reimbursements,
                        ts.ReimbursementsDescription)
                    {
                        TypeOfWork = jp == null ? cpj.OtherJobPosition : jp.Value
                    };
        var result = await query.ToListAsync();
        return result;
    }

    public async Task<List<WorkerReadyForPayStubModel>> GetWorkersReadyForPayStub(IEnumerable<Guid> agencyIds)
    {
        var timeSheet = from ts in _context.TimeSheet.Where(c => c.TimeInApproved != null && c.TimeOutApproved != null && agencyIds.Contains(c.WorkerRequest.Request.AgencyId))
                        join tstP in _context.TimeSheetTotalPayroll on ts.Id equals tstP.TimeSheetId into tmp0
                        from tstP in tmp0.DefaultIfEmpty()
                        where tstP == null
                        select ts;
        return await (from ts in timeSheet
                      join wp in _context.WorkerProfile.Where(wp => !wp.IsSubcontractor) on ts.WorkerRequest.WorkerId equals wp.WorkerId
                      join cp in _context.CompanyProfile on ts.WorkerRequest.Request.CompanyId equals cp.CompanyId
                      group new { wp } by new { wp.WorkerId, wp.FirstName, wp.MiddleName, wp.LastName, wp.SecondLastName, cp.BusinessName }
                      into result
                      select new WorkerReadyForPayStubModel
                      {
                          WorkerId = result.Key.WorkerId,
                          FirstName = result.Key.FirstName,
                          MiddleName = result.Key.MiddleName,
                          LastName = result.Key.LastName,
                          SecondLastName = result.Key.SecondLastName,
                          BusinessName = result.Key.BusinessName
                      }
            ).ToListAsync();
    }

    public async Task<TimeSheetUsagesModel> GetTimeSheetUsages(Guid requestId, Guid workerId, Guid id)
    {
        var request = await requestRepository.GetRequestDetailForAgency(requestId);
        var timesheetTotal = _context.TimeSheetTotal.Where(ts => ts.TimeSheetId == id);
        long? invoiceNumber;
        if (request.JobLocation.IsUSA)
        {
            invoiceNumber = await (from tst in timesheetTotal
                                   join iut in _context.InvoiceUSATimeSheetTotals on tst.Id equals iut.TimeSheetTotalId
                                   select iut.InvoiceUSA.InvoiceNumberId).FirstOrDefaultAsync();
        }
        else
        {
            invoiceNumber = await (from tst in timesheetTotal
                                   join it in _context.InvoiceTotals on tst.Id equals it.TimeSheetTotalId
                                   select it.Invoice.InvoiceNumber).FirstOrDefaultAsync();
        }
        var paystubNumber = await (from tst in _context.TimeSheetTotal.Where(tst => tst.TimeSheetId == id)
                                   join pswd in _context.PayStubWageDetail on tst.Id equals pswd.TimeSheetTotalId
                                   select pswd.PayStub.PayStubNumber).FirstOrDefaultAsync();
        return new TimeSheetUsagesModel
        {
            InvoiceNumber = invoiceNumber,
            PayStubNumber = paystubNumber
        };
    }

    public async Task<IEnumerable<HoursWorkedResponse>> GetHoursWorked(Guid agencyId, HoursWorkedFilter filter)
    {
        var timeSheets = _context.TimeSheet
            .AsNoTracking()
            .Include(ts => ts.WorkerRequest)
            .ThenInclude(wr => wr.Request)
            .ThenInclude(r => r.JobPositionRate)
            .ThenInclude(jpr => jpr.JobPosition)
            .Where(ts => ts.Date.Date >= filter.StartDate && ts.Date.Date <= filter.EndDate && ts.WorkerRequest.Request.AgencyId == agencyId);
        if (filter.CompanyId.HasValue)
            timeSheets = timeSheets.Where(qb => qb.WorkerRequest.Request.CompanyId == filter.CompanyId);
        if (filter.JobPositionRateId.HasValue)
            timeSheets = timeSheets.Where(qb => qb.WorkerRequest.Request.JobPositionRateId == filter.JobPositionRateId);
        var query = from ts in timeSheets
                    join wp in _context.WorkerProfile on ts.WorkerRequest.WorkerId equals wp.WorkerId
                    select new
                    {
                        wp.WorkerId,
                        WorkerName = wp.FirstName + " " + wp.LastName,
                        JobPosition = ts.WorkerRequest.Request.JobPositionRate.JobPosition != null ?
                            ts.WorkerRequest.Request.JobPositionRate.JobPosition.Value :
                            ts.WorkerRequest.Request.JobPositionRate.OtherJobPosition,
                        BillRate = ts.WorkerRequest.Request.AgencyRate,
                        RegularHoursWorked = ts.TimeSheetTotal.RegularHours.TotalHours + ts.TimeSheetTotal.OtherRegularHours.TotalHours,
                        OvertimeHoursWorked = ts.TimeSheetTotal.OvertimeHours.TotalHours,
                        HolidayHoursWorked = ts.TimeSheetTotal.HolidayHours.TotalHours,
                        NightHoursWorked = ts.TimeSheetTotal.NightShiftHours.TotalHours
                    };
        var workedHours = from q in query
                          group q by new
                          {
                              q.WorkerId,
                              q.WorkerName,
                              q.JobPosition,
                              q.BillRate
                          } into jwrwp
                          orderby jwrwp.Key.WorkerName
                          select new HoursWorkedResponse
                          {
                              WorkerName = jwrwp.Key.WorkerName,
                              JobPosition = jwrwp.Key.JobPosition,
                              BillRate = jwrwp.Key.BillRate.Value,
                              RegularHoursWorked = jwrwp.Sum(a => a.RegularHoursWorked),
                              OvertimeHoursWorked = jwrwp.Sum(a => a.OvertimeHoursWorked),
                              HolidayHoursWorked = jwrwp.Sum(a => a.HolidayHoursWorked),
                              NightHoursWorked = jwrwp.Sum(a => a.NightHoursWorked),
                              TotalPayRegularRate = jwrwp.Key.BillRate.Value * (int)jwrwp.Sum(a => a.RegularHoursWorked),
                              TotalPayOvertimeRate = jwrwp.Key.BillRate.Value * rates.OverTime * (int)jwrwp.Sum(a => a.OvertimeHoursWorked),
                              TotalPayHolidayRate = jwrwp.Key.BillRate.Value * rates.Holiday * (int)jwrwp.Sum(a => a.HolidayHoursWorked),
                              TotalPayNightRate = jwrwp.Key.BillRate.Value * rates.NightShift * (int)jwrwp.Sum(a => a.NightHoursWorked),
                          };
        var result = await workedHours.ToListAsync();
        return result;
    }

    public async Task<IEnumerable<CompanyProfileJobPositionRateModel>> GetJobPositions(Guid companyId, DateTime startDate, DateTime endDate)
    {
        var agencyCompanyProfileJobPositionRate = _context.TimeSheet
            .AsNoTracking()
            .Include(ts => ts.WorkerRequest).ThenInclude(wr => wr.Request).ThenInclude(r => r.JobPositionRate)
            .Where(ts => ts.Date.Date >= startDate && ts.Date.Date <= endDate)
            .Where(ts => ts.WorkerRequest.Request.CompanyId == companyId)
            .Select(lj => new
            {
                lj.WorkerRequest.Request.JobPositionRate.Id,
                JobPosition = lj.WorkerRequest.Request.JobPositionRate.JobPosition != null ?
                    lj.WorkerRequest.Request.JobPositionRate.JobPosition.Value :
                    lj.WorkerRequest.Request.JobPositionRate.OtherJobPosition
            });
        var result = await agencyCompanyProfileJobPositionRate.GroupBy(acpjr => new { acpjr.Id, acpjr.JobPosition })
            .Select(acpjr => new CompanyProfileJobPositionRateModel
            {
                Id = acpjr.Key.Id,
                JobPosition = new JobPositionDetailModel
                {
                    Value = acpjr.Key.JobPosition
                },
                Value = acpjr.Key.JobPosition
            }).ToListAsync();
        return result;
    }

    public async Task<TimeSheet> GetLatestTimesheet(Guid workerId, Guid requestId, DateTime from)
    {
        var timeSheet = _context.TimeSheet
            .Include(ts => ts.WorkerRequest)
            .ThenInclude(wr => wr.Request)
            .ThenInclude(r => r.JobLocation)
            .Where(ts => ts.Date.Date == from.Date);

        var query = from wp in _context.WorkerProfile.Where(wp => wp.WorkerId == workerId)
                    join wr in _context.WorkerRequest.Where(wr => wr.RequestId == requestId) on wp.WorkerId equals wr.WorkerId
                    join ts in timeSheet on wr.Id equals ts.WorkerRequestId
                    orderby ts.Date descending
                    select ts;
        return await query.FirstOrDefaultAsync();
    }

    public Task<TimeSheet> GetLatestTimesheet(Expression<Func<WorkerProfile, bool>> condition) =>
        (from wp in _context.WorkerProfile.Where(condition)
         join wr in _context.WorkerRequest.Where(c => c.WorkerRequestStatus == WorkerRequestStatus.Booked) on wp.WorkerId equals wr.WorkerId
         join ts in _context.TimeSheet on wr.Id equals ts.WorkerRequestId
         select ts).OrderByDescending(c => c.Date).FirstOrDefaultAsync();

    public async Task<TimeSheet> GetTimeSheeFromTheLast14Hours(Guid workerId, Guid requestId, DateTime now)
    {
        var last14Hours = now.AddHours(-TimeLimits.DefaultTimeLimits.MaximumHoursDay);
        var timeSheet = _context.TimeSheet
            .Where(ts => ts.ClockIn.HasValue && (ts.ClockIn.Value.Date == now.Date || ts.ClockIn.Value.Date == last14Hours.Date));
        var query = from wp in _context.WorkerProfile.Where(wp => wp.WorkerId == workerId)
                    join wr in _context.WorkerRequest.Where(wr => wr.RequestId == requestId) on wp.WorkerId equals wr.WorkerId
                    join ts in timeSheet on wr.Id equals ts.WorkerRequestId
                    orderby ts.Date descending
                    select ts;
        return await query.FirstOrDefaultAsync();
    }

    public async Task<bool> TimesheetUsedByAccounting(Guid id)
    {
        var timesheetInvoice = await _context.TimeSheetTotal.AnyAsync(ts => ts.TimeSheetId == id);
        var timsheetPayroll = await _context.TimeSheetTotalPayroll.AnyAsync(ts => ts.TimeSheetId == id);
        return timesheetInvoice && timsheetPayroll;
    }

    public Task<List<RequestTimeSheetModel>> GetRequestTimeSheet(Guid requestId)
    {
        var query = from ts in _context.TimeSheet.Where(ts => ts.WorkerRequest.Request.Id == requestId)
                    join cp in _context.CompanyProfile on ts.WorkerRequest.Request.CompanyId equals cp.CompanyId
                    join wp in _context.WorkerProfile on ts.WorkerRequest.WorkerId equals wp.WorkerId
                    select new RequestTimeSheetModel
                    {
                        CompanyFullName = cp.FullName,
                        AgencyFullName = ts.WorkerRequest.Request.Agency.FullName,
                        WorkerFullName = $"{wp.FirstName} {wp.MiddleName} {wp.LastName} {wp.SecondLastName}",
                        Date = ts.Date,
                        TimeInApproved = ts.TimeInApproved,
                        TimeOutApproved = ts.TimeOutApproved,
                        DurationBreak = ts.WorkerRequest.Request.DurationBreak,
                        ClockIn = ts.ClockIn,
                        ClockOut = ts.ClockOut,
                        Comment = ts.Comment,
                    };
        return query.ToListAsync();
    }
}