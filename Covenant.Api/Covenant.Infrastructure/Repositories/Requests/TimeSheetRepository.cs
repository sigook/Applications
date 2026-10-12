using Covenant.Common.Configuration;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Models.Request.TimeSheet.Agency;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Repositories.Requests;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Covenant.Infrastructure.Repositories.Requests;

public class TimesheetRepository : ITimesheetRepository
{
    private readonly CovenantContext _context;
    private readonly Rates rates;
    private readonly TimeLimits timeLimits;

    public TimesheetRepository(
        CovenantContext context,
        Rates rates,
        TimeLimits timeLimits)
    {
        _context = context;
        this.rates = rates;
        this.timeLimits = timeLimits;
    }

    public async Task Create<T>(T entity) where T : class => await _context.Set<T>().AddAsync(entity);

    public void Delete<T>(T entity) where T : class => _context.Set<T>().Remove(entity);

    public Task SaveChangesAsync() => _context.SaveChangesAsync();

    public Task<TimeSheet> GetTimeSheet(Guid id) => _context.TimeSheets.SingleOrDefaultAsync(s => s.Id == id);

    public async Task<IEnumerable<TimeSheet>> GetTimeSheets(Guid workerProfileId, Guid requestId, Expression<Func<Common.Entities.Request.Request, bool>> requestCondition)
    {
        var query = from wr in _context.WorkerRequests.Where(c => c.WorkerProfileId == workerProfileId && c.RequestId == requestId)
                    join r in _context.Requests.Where(requestCondition) on wr.RequestId equals r.Id
                    from ts in wr.TimeSheets
                    select ts;
        return await query.ToListAsync();
    }

    public async Task<IEnumerable<TimeSheetListModel>> GetTimeSheetsListModel(Guid workerProfileId, Guid requestId, DateTime? startDate, DateTime? endDate)
    {
        var timeSheet = _context.TimeSheets.AsQueryable();
        if (startDate.HasValue)
        {
            timeSheet = timeSheet.Where(ts => ts.Date >= startDate.Value);
        }
        if (endDate.HasValue)
        {
            timeSheet = timeSheet.Where(ts => ts.Date <= endDate.Value);
        }
        var query = from wr in _context.WorkerRequests.Where(c => c.WorkerProfileId == workerProfileId && c.RequestId == requestId)
                    join ts in timeSheet on wr.Id equals ts.WorkerRequestId
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
                        CanUpdate = ts.TimeSheetTotal == null && ts.TimeSheetTotalPayroll == null,
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

    public async Task<PaginatedList<TimeSheetHistoryModel>> GetTimeSheetHistory(Guid agencyId, Guid workerProfileId, GetWorkerTimeSheetHistoryFilter filter)
    {
        var query = _context.TimeSheets
            .Where(ts => ts.WorkerRequest.WorkerProfileId == workerProfileId && ts.WorkerRequest.WorkerProfile.AgencyId == agencyId)
            .Select(ts => new TimeSheetHistoryModel
            {
                Id = ts.Id,
                RequestId = ts.WorkerRequest.RequestId,
                NumberId = ts.WorkerRequest.Request.NumberId,
                BusinessName = ts.WorkerRequest.Request.CompanyProfile.FullName,
                JobTitle = ts.WorkerRequest.Request.JobTitle,
                Date = ts.Date,
                IsHoliday = ts.IsHoliday,
                RegularHours = ts.TimeSheetTotal == null ? 0 : ts.TimeSheetTotal.RegularHours.TotalHours,
                HolidayHours = ts.TimeSheetTotal == null ? 0 : ts.TimeSheetTotal.HolidayHours.TotalHours,
                OvertimeHours = ts.TimeSheetTotal == null ? 0 : ts.TimeSheetTotal.OvertimeHours.TotalHours,
                MissingHours = ts.MissingHours.TotalHours,
                MissingHoursOvertime = ts.MissingHoursOvertime.TotalHours
            });
        query = query.Where(ApplyFilterTimeSheetHistory(filter));
        query = ApplySortTimeSheetHistory(query, filter);
        var page = await query.ToPaginatedList(filter);
        if (page.Items.Any())
        {
            await FillTimeSheetHistoryAccumulated(workerProfileId, page.Items);
        }
        return page;
    }

    private async Task FillTimeSheetHistoryAccumulated(Guid workerProfileId, List<TimeSheetHistoryModel> page)
    {
        var history = await _context.TimeSheets
            .Where(t => t.WorkerRequest.WorkerProfileId == workerProfileId)
            .OrderBy(t => t.Date).ThenBy(t => t.Id)
            .Select(t => new TimeSheetHistoryHours(
                t.Id,
                t.TimeSheetTotal == null ? 0 : t.TimeSheetTotal.RegularHours.TotalHours,
                t.TimeSheetTotal == null ? 0 : t.TimeSheetTotal.HolidayHours.TotalHours,
                t.TimeSheetTotal == null ? 0 : t.TimeSheetTotal.OvertimeHours.TotalHours,
                t.MissingHours.TotalHours,
                t.MissingHoursOvertime.TotalHours))
            .ToListAsync();
        var byId = page.ToDictionary(t => t.Id);
        double regular = 0, holiday = 0, overtime = 0, missing = 0, missingOvertime = 0;
        foreach (var timeSheet in history)
        {
            regular += timeSheet.Regular;
            holiday += timeSheet.Holiday;
            overtime += timeSheet.Overtime;
            missing += timeSheet.Missing;
            missingOvertime += timeSheet.MissingOvertime;
            if (!byId.TryGetValue(timeSheet.Id, out var row)) continue;
            row.AccumulatedRegularHours = regular;
            row.AccumulatedHolidayHours = holiday;
            row.AccumulatedOvertimeHours = overtime;
            row.AccumulatedMissingHours = missing;
            row.AccumulatedMissingHoursOvertime = missingOvertime;
        }
    }

    private sealed record TimeSheetHistoryHours(Guid Id, double Regular, double Holiday, double Overtime, double Missing, double MissingOvertime);

    private static Expression<Func<TimeSheetHistoryModel, bool>> ApplyFilterTimeSheetHistory(GetWorkerTimeSheetHistoryFilter filter)
    {
        var predicate = PredicateBuilder.New<TimeSheetHistoryModel>(true);
        if (!string.IsNullOrWhiteSpace(filter.CompanyName))
        {
            var companyName = filter.CompanyName.ToLower();
            predicate = predicate.And(t => t.BusinessName.ToLower().Contains(companyName));
        }
        if (filter.NumberId.HasValue)
            predicate = predicate.And(t => t.NumberId == filter.NumberId.Value);
        if (filter.DateFrom.HasValue && filter.DateTo.HasValue)
            predicate = predicate.And(t => t.Date.Date >= filter.DateFrom.Value.Date && t.Date.Date <= filter.DateTo.Value.Date);
        return predicate;
    }

    private static IQueryable<TimeSheetHistoryModel> ApplySortTimeSheetHistory(IQueryable<TimeSheetHistoryModel> query, GetWorkerTimeSheetHistoryFilter filter)
    {
        switch (filter.SortBy)
        {
            case WorkerTimeSheetHistorySortBy.Company:
                return query.AddOrderBy(filter, t => t.BusinessName).ThenByDescending(t => t.Date).ThenBy(t => t.Id);
            case WorkerTimeSheetHistorySortBy.Request:
                return query.AddOrderBy(filter, t => t.NumberId).ThenByDescending(t => t.Date).ThenBy(t => t.Id);
            case WorkerTimeSheetHistorySortBy.Date:
                return filter.IsDescending
                    ? query.OrderByDescending(t => t.Date).ThenByDescending(t => t.Id)
                    : query.OrderBy(t => t.Date).ThenBy(t => t.Id);
            default:
                return query.OrderByDescending(t => t.Date).ThenByDescending(t => t.Id);
        }
    }

    public async Task<PaginatedList<TimeSheetListModel>> GetTimeSheetsForWorker(Guid workerId, Guid requestId, Pagination pagination)
    {
        var query = (from workerRequest in _context.WorkerRequests
                     from timeSheet in workerRequest.TimeSheets
                     where workerRequest.WorkerProfile.WorkerId == workerId && workerRequest.RequestId == requestId
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
        var timeSheet = _context.TimeSheets
            .Include(ts => ts.WorkerRequest)
            .ThenInclude(ts => ts.Request)
            .ThenInclude(ts => ts.CompanyProfile).ThenInclude(cp => cp.Agency)
            .Where(ts => agencyIds.Contains(ts.WorkerRequest.Request.CompanyProfile.AgencyId) && ts.WorkerRequest.Request.CompanyProfileId == model.CompanyProfileId)
            .Where(ts => ts.TimeInApproved != null && ts.TimeOutApproved != null && ts.TimeSheetTotal == null);
        if (model.RequestIds != null && model.RequestIds.Any())
        {
            timeSheet = timeSheet.Where(ts => model.RequestIds.Contains(ts.WorkerRequest.RequestId));
        }
        if (model.From.HasValue && model.To.HasValue)
        {
            timeSheet = timeSheet.Where(ts => ts.Date.Date >= model.From.Value && ts.Date.Date <= model.To.Value);
        }
        var query = from ts in timeSheet
                    select new TimeSheetApprovedBillingModel
                    {
                        PaidHolidays = ts.WorkerRequest.Request.CompanyProfile.PaidHolidays,
                        OvertimeStartsAfter = ts.WorkerRequest.Request.JobPositionRate != null && ts.WorkerRequest.Request.JobPositionRate.OvertimeStartsAfter.HasValue
                            ? ts.WorkerRequest.Request.JobPositionRate.OvertimeStartsAfter.Value
                            : (ts.WorkerRequest.Request.JobLocation.City.Province.ProvinceSetting != null &&
                                ts.WorkerRequest.Request.JobLocation.City.Province.ProvinceSetting.OvertimeStartsAfter.HasValue
                                ? ts.WorkerRequest.Request.JobLocation.City.Province.ProvinceSetting.OvertimeStartsAfter.Value
                                : ts.WorkerRequest.Request.CompanyProfile.OvertimeStartsAfter),
                        RequestId = ts.WorkerRequest.RequestId,
                        JobTitle = string.IsNullOrWhiteSpace(ts.WorkerRequest.Request.BillingTitle) ? ts.WorkerRequest.Request.JobTitle : ts.WorkerRequest.Request.BillingTitle,
                        AgencyRate = ts.WorkerRequest.Request.AgencyRate.Value,
                        BreakIsPaid = ts.WorkerRequest.Request.BreakIsPaid,
                        DurationBreak = ts.WorkerRequest.Request.DurationBreak,
                        HolidayIsPaid = ts.WorkerRequest.Request.HolidayIsPaid,
                        WorkerId = ts.WorkerRequest.WorkerProfile.WorkerId,
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
                        Holiday = rates.Holiday,
                        CountryCode = ts.WorkerRequest.Request.JobLocation.City.Province.Country.Code,
                        Tax = ts.WorkerRequest.Request.JobLocation.LocationTax != null
                            ? ts.WorkerRequest.Request.JobLocation.LocationTax.Tax1
                            : 0m
                    };
        var result = await query.ToListAsync();
        return result;
    }

    public async Task<List<TimeSheetApprovedPayrollModel>> GetTimeSheetForCreatingReportsSubcontractor(IEnumerable<Guid> agencyIds, Guid companyProfileId)
    {
        var timeSheets = _context.TimeSheets
            .Where(ts => ts.TimeInApproved != null && ts.TimeOutApproved != null && ts.TimeSheetTotalPayroll == null)
            .Where(ts => agencyIds.Contains(ts.WorkerRequest.Request.CompanyProfile.AgencyId))
            .Where(ts => ts.WorkerRequest.Request.CompanyProfileId == companyProfileId);

        var query = from ts in timeSheets
                    where ts.WorkerRequest.WorkerProfile.IsSubcontractor
                    orderby ts.Date
                    select new TimeSheetApprovedPayrollModel(
                        ts.WorkerRequest.Request.JobPositionRate != null && ts.WorkerRequest.Request.JobPositionRate.OvertimeStartsAfter.HasValue
                            ? ts.WorkerRequest.Request.JobPositionRate.OvertimeStartsAfter.Value
                            : (ts.WorkerRequest.Request.JobLocation.City.Province.ProvinceSetting != null && ts.WorkerRequest.Request.JobLocation.City.Province.ProvinceSetting.OvertimeStartsAfter.HasValue
                                ? ts.WorkerRequest.Request.JobLocation.City.Province.ProvinceSetting.OvertimeStartsAfter.Value
                                : ts.WorkerRequest.Request.CompanyProfile.OvertimeStartsAfter),
                        ts.WorkerRequest.RequestId,
                        ts.WorkerRequest.Request.WorkerRate.Value,
                        ts.WorkerRequest.Request.BreakIsPaid,
                        ts.WorkerRequest.Request.DurationBreak,
                        ts.WorkerRequest.Request.JobLocation.City.Province.ProvinceSetting != null && ts.WorkerRequest.Request.JobLocation.City.Province.ProvinceSetting.PaidHolidays.HasValue
                            ? ts.WorkerRequest.Request.JobLocation.City.Province.ProvinceSetting.PaidHolidays.Value
                            : ts.WorkerRequest.Request.CompanyProfile.PaidHolidays,
                        ts.WorkerRequest.WorkerProfile.WorkerId,
                        ts.WorkerRequest.WorkerProfileId,
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
                        TypeOfWork = ts.WorkerRequest.Request.JobPositionRate.JobPosition,
                        CountryCode = ts.WorkerRequest.Request.JobLocation.City.Province.Country.Code
                    };
        var result = await query.ToListAsync();
        return result;

    }

    public async Task<List<TimeSheetApprovedPayrollModel>> GetTimeSheetForCreatingPayStubs(IEnumerable<Guid> agencyIds, Guid workerId)
    {
        var timeSheets = _context.TimeSheets
            .Where(ts => ts.TimeInApproved != null && ts.TimeOutApproved != null && ts.TimeSheetTotalPayroll == null)
            .Where(ts => agencyIds.Contains(ts.WorkerRequest.Request.CompanyProfile.AgencyId))
            .Where(ts => ts.WorkerRequest.WorkerProfile.WorkerId == workerId);

        var query = from ts in timeSheets
                    orderby ts.Date
                    select new TimeSheetApprovedPayrollModel(
                        ts.WorkerRequest.Request.JobPositionRate != null && ts.WorkerRequest.Request.JobPositionRate.OvertimeStartsAfter.HasValue
                            ? ts.WorkerRequest.Request.JobPositionRate.OvertimeStartsAfter.Value
                            : (ts.WorkerRequest.Request.JobLocation.City.Province.ProvinceSetting != null && ts.WorkerRequest.Request.JobLocation.City.Province.ProvinceSetting.OvertimeStartsAfter.HasValue
                                ? ts.WorkerRequest.Request.JobLocation.City.Province.ProvinceSetting.OvertimeStartsAfter.Value
                                : ts.WorkerRequest.Request.CompanyProfile.OvertimeStartsAfter),
                        ts.WorkerRequest.RequestId,
                        ts.WorkerRequest.Request.WorkerRate.Value,
                        ts.WorkerRequest.Request.BreakIsPaid,
                        ts.WorkerRequest.Request.DurationBreak,
                        ts.WorkerRequest.Request.JobLocation.City.Province.ProvinceSetting != null && ts.WorkerRequest.Request.JobLocation.City.Province.ProvinceSetting.PaidHolidays.HasValue
                            ? ts.WorkerRequest.Request.JobLocation.City.Province.ProvinceSetting.PaidHolidays.Value
                            : ts.WorkerRequest.Request.CompanyProfile.PaidHolidays,
                        ts.WorkerRequest.WorkerProfile.WorkerId,
                        ts.WorkerRequest.WorkerProfileId,
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
                        TypeOfWork = ts.WorkerRequest.Request.JobPositionRate.JobPosition,
                        CountryCode = ts.WorkerRequest.Request.JobLocation.City.Province.Country.Code
                    };
        var result = await query.ToListAsync();
        return result;
    }

    public async Task<List<TimeSheetApprovedPayrollModel>> GetApprovedTimeSheetsInRange(Guid workerProfileId, DateTime start, DateTime end)
    {
        var timeSheets = _context.TimeSheets
            .Where(ts => ts.TimeInApproved != null && ts.TimeOutApproved != null)
            .Where(ts => ts.Date.Date >= start.Date && ts.Date.Date <= end.Date);

        var query = from ts in timeSheets
                    where ts.WorkerRequest.WorkerProfileId == workerProfileId
                          && !ts.WorkerRequest.WorkerProfile.IsSubcontractor
                          && ts.WorkerRequest.Request.WorkerRate != null && !ts.WorkerRequest.Request.WorkerSalary.HasValue
                    orderby ts.Date
                    select new TimeSheetApprovedPayrollModel(
                        ts.WorkerRequest.Request.JobPositionRate != null && ts.WorkerRequest.Request.JobPositionRate.OvertimeStartsAfter.HasValue
                            ? ts.WorkerRequest.Request.JobPositionRate.OvertimeStartsAfter.Value
                            : (ts.WorkerRequest.Request.JobLocation.City.Province.ProvinceSetting != null && ts.WorkerRequest.Request.JobLocation.City.Province.ProvinceSetting.OvertimeStartsAfter.HasValue
                                ? ts.WorkerRequest.Request.JobLocation.City.Province.ProvinceSetting.OvertimeStartsAfter.Value
                                : ts.WorkerRequest.Request.CompanyProfile.OvertimeStartsAfter),
                        ts.WorkerRequest.RequestId,
                        ts.WorkerRequest.Request.WorkerRate.Value,
                        ts.WorkerRequest.Request.BreakIsPaid,
                        ts.WorkerRequest.Request.DurationBreak,
                        ts.WorkerRequest.Request.JobLocation.City.Province.ProvinceSetting != null && ts.WorkerRequest.Request.JobLocation.City.Province.ProvinceSetting.PaidHolidays.HasValue
                            ? ts.WorkerRequest.Request.JobLocation.City.Province.ProvinceSetting.PaidHolidays.Value
                            : ts.WorkerRequest.Request.CompanyProfile.PaidHolidays,
                        ts.WorkerRequest.WorkerProfile.WorkerId,
                        ts.WorkerRequest.WorkerProfileId,
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
                        CountryCode = ts.WorkerRequest.Request.JobLocation.City.Province.Country.Code
                    };
        return await query.ToListAsync();
    }

    public async Task<List<WorkerReadyForPayStubModel>> GetWorkersReadyForPayStub(IEnumerable<Guid> agencyIds)
    {
        var timeSheet = _context.TimeSheets.Where(c => c.TimeInApproved != null
                                                      && c.TimeOutApproved != null
                                                      && agencyIds.Contains(c.WorkerRequest.Request.CompanyProfile.AgencyId)
                                                      && c.TimeSheetTotalPayroll == null);
        return await (from ts in timeSheet
                      where !ts.WorkerRequest.WorkerProfile.IsSubcontractor
                      group ts by new
                      {
                          ts.WorkerRequest.WorkerProfile.WorkerId,
                          ts.WorkerRequest.WorkerProfile.FirstName,
                          ts.WorkerRequest.WorkerProfile.MiddleName,
                          ts.WorkerRequest.WorkerProfile.LastName,
                          ts.WorkerRequest.WorkerProfile.SecondLastName,
                          ts.WorkerRequest.Request.CompanyProfile.FullName
                      }
                      into result
                      select new WorkerReadyForPayStubModel
                      {
                          WorkerId = result.Key.WorkerId,
                          FirstName = result.Key.FirstName,
                          MiddleName = result.Key.MiddleName,
                          LastName = result.Key.LastName,
                          SecondLastName = result.Key.SecondLastName,
                          BusinessName = result.Key.FullName
                      }
            ).ToListAsync();
    }

    public async Task<TimeSheetUsagesModel> GetTimeSheetUsages(Guid requestId, Guid workerProfileId, Guid id)
    {
        var invoiceNumber = await _context.InvoiceTotals
            .Where(it => it.TimeSheetTotal.TimeSheetId == id)
            .Select(it => it.Invoice.InvoiceNumber)
            .FirstOrDefaultAsync();
        var paystubNumber = await (from tst in _context.TimeSheetTotals.Where(tst => tst.TimeSheetId == id)
                                   join pswd in _context.PayStubWageDetails on tst.Id equals pswd.TimeSheetTotalId
                                   select pswd.PayStub.PayStubNumber).FirstOrDefaultAsync();
        return new TimeSheetUsagesModel
        {
            InvoiceNumber = invoiceNumber,
            PayStubNumber = paystubNumber
        };
    }

    public async Task<IEnumerable<HoursWorkedResponse>> GetHoursWorked(Guid agencyId, HoursWorkedFilter filter)
    {
        var timeSheets = _context.TimeSheets
            .AsNoTracking()
            .Include(ts => ts.WorkerRequest)
            .ThenInclude(wr => wr.Request)
            .ThenInclude(r => r.JobPositionRate)
            .Where(ts => ts.Date.Date >= filter.StartDate && ts.Date.Date <= filter.EndDate && ts.WorkerRequest.Request.CompanyProfile.AgencyId == agencyId);
        if (filter.CompanyProfileId.HasValue)
            timeSheets = timeSheets.Where(qb => qb.WorkerRequest.Request.CompanyProfileId == filter.CompanyProfileId);
        if (filter.JobPositionRateId.HasValue)
            timeSheets = timeSheets.Where(qb => qb.WorkerRequest.Request.JobPositionRateId == filter.JobPositionRateId);
        var query = from ts in timeSheets
                    select new
                    {
                        ts.WorkerRequest.WorkerProfile.WorkerId,
                        WorkerName = ts.WorkerRequest.WorkerProfile.FirstName + " " + ts.WorkerRequest.WorkerProfile.LastName,
                        JobPosition = ts.WorkerRequest.Request.JobPositionRate.JobPosition,
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

    public async Task<IEnumerable<TimesheetsReportResponse>> GetTimesheetsReport(Guid agencyId, TimesheetsReportFilter filter)
    {
        var timeSheets = _context.TimeSheets
            .AsNoTracking()
            .Include(ts => ts.WorkerRequest)
            .ThenInclude(wr => wr.Request)
            .Where(ts => ts.Date.Date >= filter.StartDate && ts.Date.Date <= filter.EndDate && ts.WorkerRequest.Request.CompanyProfile.AgencyId == agencyId);
        var query = from ts in timeSheets
                    select new
                    {
                        ts.WorkerRequest.WorkerProfile.WorkerId,
                        ts.WorkerRequest.WorkerProfile.ExternalId,
                        FullName = ts.WorkerRequest.WorkerProfile.LastName + ", " + ts.WorkerRequest.WorkerProfile.FirstName,
                        ts.WorkerRequest.WorkerProfile.SocialInsurance,
                        ts.WorkerRequest.WorkerProfile.WcCode,
                        Client = ts.WorkerRequest.Request.CompanyProfile.FullName,
                        JobName = ts.WorkerRequest.Request.JobCosting,
                        PayRate = ts.WorkerRequest.Request.WorkerRate,
                        RegularHours = ts.TimeSheetTotal.RegularHours.TotalHours + ts.TimeSheetTotal.OtherRegularHours.TotalHours,
                        OvertimeHours = ts.TimeSheetTotal.OvertimeHours.TotalHours
                    };
        var report = from q in query
                     group q by new
                     {
                         q.WorkerId,
                         q.ExternalId,
                         q.FullName,
                         q.SocialInsurance,
                         q.WcCode,
                         q.Client,
                         q.JobName,
                         q.PayRate
                     } into g
                     orderby g.Key.FullName
                     select new TimesheetsReportResponse
                     {
                         EmployeeId = g.Key.ExternalId,
                         FullName = g.Key.FullName,
                         SocialInsurance = g.Key.SocialInsurance,
                         WcCode = g.Key.WcCode,
                         Client = g.Key.Client,
                         JobName = g.Key.JobName,
                         PayRate = g.Key.PayRate ?? 0,
                         RegularHours = g.Sum(a => a.RegularHours),
                         OvertimeHours = g.Sum(a => a.OvertimeHours)
                     };
        var result = await report.ToListAsync();
        return result;
    }

    public async Task<IEnumerable<CompanyProfileJobPositionRateModel>> GetJobPositions(Guid companyProfileId, DateTime startDate, DateTime endDate)
    {
        var agencyCompanyProfileJobPositionRate = _context.TimeSheets
            .AsNoTracking()
            .Include(ts => ts.WorkerRequest).ThenInclude(wr => wr.Request).ThenInclude(r => r.JobPositionRate)
            .Where(ts => ts.Date.Date >= startDate && ts.Date.Date <= endDate)
            .Where(ts => ts.WorkerRequest.Request.CompanyProfileId == companyProfileId)
            .Select(lj => new
            {
                lj.WorkerRequest.Request.JobPositionRate.Id,
                JobPosition = lj.WorkerRequest.Request.JobPositionRate.JobPosition
            });
        var result = await agencyCompanyProfileJobPositionRate.GroupBy(acpjr => new { acpjr.Id, acpjr.JobPosition })
            .Select(acpjr => new CompanyProfileJobPositionRateModel
            {
                Id = acpjr.Key.Id,
                JobPosition = acpjr.Key.JobPosition
            }).ToListAsync();
        return result;
    }

    public async Task<TimeSheet> GetLatestTimesheet(Guid workerId, Guid requestId, DateTime from)
    {
        var timeSheet = _context.TimeSheets
            .Include(ts => ts.WorkerRequest)
            .ThenInclude(wr => wr.Request)
            .ThenInclude(r => r.JobLocation)
            .Where(ts => ts.Date.Date == from.Date);

        var query = from ts in timeSheet
                    where ts.WorkerRequest.RequestId == requestId
                          && ts.WorkerRequest.WorkerProfile.WorkerId == workerId
                    orderby ts.Date descending
                    select ts;
        return await query.FirstOrDefaultAsync();
    }

    public Task<TimeSheet> GetLatestTimesheet(Expression<Func<WorkerProfile, bool>> condition) =>
        (from wp in _context.WorkerProfiles.Where(condition)
         join wr in _context.WorkerRequests.Where(c => c.WorkerRequestStatus == WorkerRequestStatus.Booked) on wp.Id equals wr.WorkerProfileId
         from ts in wr.TimeSheets
         select ts).OrderByDescending(c => c.Date).FirstOrDefaultAsync();

    public async Task<TimeSheet> GetTimeSheeFromTheLast14Hours(Guid workerId, Guid requestId, DateTime now)
    {
        var last14Hours = now.AddHours(-TimeLimits.DefaultTimeLimits.MaximumHoursDay);
        var timeSheet = _context.TimeSheets
            .Where(ts => ts.ClockIn.HasValue && (ts.ClockIn.Value.Date == now.Date || ts.ClockIn.Value.Date == last14Hours.Date));
        var query = from ts in timeSheet
                    where ts.WorkerRequest.RequestId == requestId
                          && ts.WorkerRequest.WorkerProfile.WorkerId == workerId
                    orderby ts.Date descending
                    select ts;
        return await query.FirstOrDefaultAsync();
    }

    public async Task<bool> TimesheetUsedByAccounting(Guid id)
    {
        var timesheetInvoice = await _context.TimeSheetTotals.AnyAsync(ts => ts.TimeSheetId == id);
        var timsheetPayroll = await _context.TimeSheetTotalPayrolls.AnyAsync(ts => ts.TimeSheetId == id);
        return timesheetInvoice && timsheetPayroll;
    }

    public Task<List<RequestTimeSheetModel>> GetRequestTimeSheet(Guid requestId)
    {
        var query = from ts in _context.TimeSheets.Where(ts => ts.WorkerRequest.Request.Id == requestId)
                    select new RequestTimeSheetModel
                    {
                        CompanyFullName = ts.WorkerRequest.Request.CompanyProfile.FullName,
                        AgencyFullName = ts.WorkerRequest.Request.CompanyProfile.Agency.FullName,
                        WorkerFullName = ts.WorkerRequest.WorkerProfile.FirstName + " " + ts.WorkerRequest.WorkerProfile.MiddleName + " " + ts.WorkerRequest.WorkerProfile.LastName + " " + ts.WorkerRequest.WorkerProfile.SecondLastName,
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