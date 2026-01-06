using Covenant.Common.Configuration;
using Covenant.Common.Entities.Request;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Request;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Resources;
using Covenant.Core.BL.Interfaces;
using Covenant.Documents.Services;
using GeoCoordinatePortable;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Covenant.Core.BL.Services;

public class TimesheetService : ITimesheetService
{
    private readonly ITimeService timeService;
    private readonly IWorkerRequestRepository workerRequestRepository;
    private readonly ITimeSheetRepository timeSheetRepository;
    private readonly ICatalogRepository catalogRepository;
    private readonly IConfiguration configuration;
    private readonly IIdentityServerService identityServerService;
    private readonly IMediator mediator;
    private readonly ILogger<TimesheetService> logger;

    public TimesheetService(
        ITimeService timeService,
        IWorkerRequestRepository workerRequestRepository,
        ITimeSheetRepository timeSheetRepository,
        ICatalogRepository catalogRepository,
        IConfiguration configuration,
        IIdentityServerService identityServerService,
        IMediator mediator,
        ILogger<TimesheetService> logger)
    {
        this.timeService = timeService;
        this.workerRequestRepository = workerRequestRepository;
        this.timeSheetRepository = timeSheetRepository;
        this.catalogRepository = catalogRepository;
        this.configuration = configuration;
        this.identityServerService = identityServerService;
        this.mediator = mediator;
        this.logger = logger;
    }

    public async Task<Result<RegisterTimeSheetResultModel>> AddClockIn(Guid requestId, Guid workerId, TimeSpan clockIn)
    {
        DateTime now = timeService.GetCurrentDateTime();
        var clockInDate = new DateTime(now.Year, now.Month, now.Day, clockIn.Hours, clockIn.Minutes, default);
        if (clockInDate > now)
            return Result.Fail<RegisterTimeSheetResultModel>($"Clock in must be less than {now:t}");

        var entity = await workerRequestRepository.GetWorkerRequest(workerId, requestId);
        if (entity is null) return Result.Fail<RegisterTimeSheetResultModel>("Worker not found");
        if (entity.IsRejected) return Result.Fail<RegisterTimeSheetResultModel>("Worker is rejected");
        if (entity.TimeSheets.Any(a => a.Date == clockInDate.Date))
            return Result.Fail<RegisterTimeSheetResultModel>("Worker already clock in");

        var result = TimeSheet.WorkerClockIn(entity.Id, clockInDate, await catalogRepository.IsHoliday(clockInDate, entity.Request.JobLocation.City.Province.Country.Code));
        if (!result) return Result.Fail<RegisterTimeSheetResultModel>(result.Errors);
        await timeSheetRepository.Create(result.Value);
        await timeSheetRepository.SaveChangesAsync();
        return Result.Ok(new RegisterTimeSheetResultModel(result.Value.Id, default, false));
    }

    public async Task<Result<Guid>> CreateTimesheet(Guid workerId, Guid requestId, TimeSheetModel timeSheetModel)
    {
        var createdBy = identityServerService.GetNickname();
        var now = timeService.GetCurrentDateTime();
        var workerRequest = await workerRequestRepository.GetWorkerRequest(workerId, requestId);
        if (workerRequest is null)
        {
            return Result.Fail<Guid>(ApiResources.InvalidRequest);
        }
        if (workerRequest.IsRejected)
        {
            DateTime? limitDateToAddTimeSheet = workerRequest.LimitDateToAddTimeSheet;
            if (!limitDateToAddTimeSheet.HasValue) return Result.Fail<Guid>("Worker is rejected");
            if (now > limitDateToAddTimeSheet) return Result.Fail<Guid>("Worker is rejected");
        }
        var isHoliday = await catalogRepository.IsHoliday(timeSheetModel.TimeIn, workerRequest.Request.JobLocation.City.Province.Country.Code);
        var totalHours = timeSheetModel.Hours;
        var rTimeSheet = TimeSheet.CreateTimeSheet(workerRequest.Id, timeSheetModel.TimeIn, totalHours, isHoliday, createdBy, now);
        if (!rTimeSheet)
        {
            return Result.Fail<Guid>(rTimeSheet.Errors);
        }
        var timeSheet = rTimeSheet.Value;
        if (workerRequest.ContainsTimeSheet(timeSheet))
        {
            return Result.Fail<Guid>($"TimeSheet for the date {timeSheet.Date:D} was already created");
        }
        timeSheet.MissingHours = timeSheetModel.MissingHours;
        timeSheet.MissingHoursOvertime = timeSheetModel.MissingHoursOvertime;
        timeSheet.MissingRateWorker = timeSheetModel.MissingRateWorker;
        timeSheet.MissingRateAgency = timeSheetModel.MissingRateAgency;
        var rDeductions = timeSheet.AddDeductionsOthers(timeSheetModel.DeductionsOthers, timeSheetModel.DeductionsOthersDescription);
        if (!rDeductions)
        {
            return Result.Fail<Guid>(rDeductions.Errors);
        }
        timeSheet.AddBonusOrOthers(timeSheetModel.BonusOrOthers, timeSheetModel.BonusOrOthersDescription);
        timeSheet.AddReimbursements(timeSheetModel.Reimbursements, timeSheetModel.ReimbursementsDescription);
        await timeSheetRepository.Create(timeSheet);
        await timeSheetRepository.SaveChangesAsync();
        return Result.Ok(timeSheet.Id);
    }

    public async Task<Result> UpdateTimesheet(Guid timeSheetId, TimeSheetModel timeSheetModel)
    {
        var updatedBy = identityServerService.GetNickname();
        var timeSheet = await timeSheetRepository.GetTimeSheet(timeSheetId);
        if (timeSheet is null)
        {
            return Result.Fail();
        }
        var timeInApproved = timeSheetModel.TimeIn;
        var timeOutApproved = timeSheetModel.TimeIn.Add(timeSheetModel.Hours);
        var result = timeSheet.AddApprovedTime(timeInApproved, timeOutApproved);
        if (!result)
        {
            return result;
        }
        timeSheet.MissingHours = timeSheetModel.MissingHours;
        timeSheet.MissingHoursOvertime = timeSheetModel.MissingHoursOvertime;
        timeSheet.MissingRateAgency = timeSheetModel.MissingRateAgency;
        timeSheet.MissingRateWorker = timeSheetModel.MissingRateWorker;
        result = timeSheet.AddDeductionsOthers(timeSheetModel.DeductionsOthers, timeSheetModel.DeductionsOthersDescription);
        if (!result)
        {
            return result;
        }
        timeSheet.AddBonusOrOthers(timeSheetModel.BonusOrOthers, timeSheetModel.BonusOrOthersDescription);
        timeSheet.AddReimbursements(timeSheetModel.Reimbursements, timeSheetModel.ReimbursementsDescription);
        timeSheet.Comment = $"Updated and approved by {updatedBy}";
        await timeSheetRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<RegisterTimeSheetResultModel>> Register(Guid requestId, WorkerLocationModel workerLocationModel)
    {
        var now = timeService.GetCurrentDateTimeOffset();
        var workerId = identityServerService.GetUserId();
        var info = await workerRequestRepository.GetWorkerRequestInfo(workerId, requestId, now.DateTime);
        Result<RegisterTimeSheetResultModel> result;
        if (configuration.GetValue<bool>("ValidateLocation"))
        {
            if (workerLocationModel.Latitude.HasValue && workerLocationModel.Longitude.HasValue)
            {
                if (info != null && info.Latitude.HasValue && info.Longitude.HasValue)
                {
                    var pinJob = new GeoCoordinate(info.Latitude.Value, info.Longitude.Value);
                    now = timeService.GetCurrentLocalDateTime(info.Latitude.Value, info.Longitude.Value);
                    var pinWorker = new GeoCoordinate(workerLocationModel.Latitude.Value, workerLocationModel.Longitude.Value);
                    var distanceBetween = pinJob.GetDistanceTo(pinWorker);
                    logger.LogWarning("{0} - {1}: Distance between is: {2}: Request Latitude: {3} Request Longitude: {4} / Worker Latitude: {5} Worker Longitude: {6}",
                        requestId, workerId, distanceBetween, info.Latitude.Value, info.Longitude.Value, workerLocationModel.Latitude.Value, workerLocationModel.Longitude.Value);
                    if (distanceBetween >= 101)
                    {
                        result = Result.Fail<RegisterTimeSheetResultModel>("You are too far from check point. Please get closer.");
                        return result;
                    }
                }
                else
                {
                    result = Result.Fail<RegisterTimeSheetResultModel>("Request doesn't have location to compare to.");
                    return result;
                }
            }
            else
            {
                result = Result.Fail<RegisterTimeSheetResultModel>("Your location is invalid. Allow app to get your location.");
                return result;
            }
        }
        var timeSheet = await timeSheetRepository.GetTimeSheeFromTheLast14Hours(workerId, requestId, now.DateTime);
        if (timeSheet == null)
        {
            result = await ClockIn(now, info);
        }
        else if (timeSheet.TimeInApproved.HasValue || timeSheet.TimeOutApproved.HasValue)
        {
            result = Result.Fail<RegisterTimeSheetResultModel>($"Time for date: {timeSheet.Date:MM/dd/yyyy} was already approved");
        }
        else if (timeSheet.IsClockOutValid(now.DateTime))
        {
            result = await ClockOut(now, info, timeSheet);
        }
        else
        {
            result = await ClockIn(now, info);
        }
        return result;
    }

    public async Task<HoursWorkedResume> GetHoursWorked(HoursWorkedFilter filter)
    {
        var agencyId = identityServerService.GetAgencyId();
        var result = await timeSheetRepository.GetHoursWorked(agencyId, filter);
        var resume = new HoursWorkedResume
        {
            TotalRegularHours = result.Sum(x => x.RegularHoursWorked),
            TotalOvertimeHours = result.Sum(x => x.OvertimeHoursWorked),
            TotalHolidayHours = result.Sum(x => x.HolidayHoursWorked),
            TotalNightHours = result.Sum(x => x.NightHoursWorked),
            TotalHours = result.Sum(x => x.TotalHoursWorked),
            TotalPayRegular = result.Sum(x => x.TotalPayRegularRate),
            TotalPayOvertime = result.Sum(x => x.TotalPayOvertimeRate),
            TotalPayHoliday = result.Sum(x => x.TotalPayHolidayRate),
            TotalPayNight = result.Sum(x => x.TotalPayNightRate),
            TotalPay = result.Sum(x => x.TotalPayRate),
            Detail = result
        };
        return resume;
    }

    public async Task<ResultGenerateDocument<MemoryStream>> GetHoursWorkedFile(HoursWorkedFilter filter)
    {
        var result = await GetHoursWorked(filter);
        var request = await mediator.Send(new GenerateHoursWorkedReport(result.Detail.ToList()));
        return request;
    }

    private async Task<Result<RegisterTimeSheetResultModel>> ClockIn(DateTimeOffset now, WorkerRequestInfoModel info)
    {
        var isHoliday = await catalogRepository.IsHoliday(now.DateTime, info.CountryCode);
        var rClockIn = TimeSheet.WorkerClockIn(info.WorkerRequestId, now.DateTime, isHoliday);
        if (!rClockIn) return Result.Fail<RegisterTimeSheetResultModel>(rClockIn.Errors);
        await timeSheetRepository.Create(rClockIn.Value);
        await timeSheetRepository.SaveChangesAsync();
        return Result.Ok(new RegisterTimeSheetResultModel(rClockIn.Value.Id, info.WorkerFullName, false));
    }

    private async Task<Result<RegisterTimeSheetResultModel>> ClockOut(DateTimeOffset now, WorkerRequestInfoModel info, TimeSheet latestTimesheet)
    {
        Result rClockOut = latestTimesheet.AddClockOut(now.DateTime);
        if (!rClockOut) return Result.Fail<RegisterTimeSheetResultModel>(rClockOut.Errors);
        await timeSheetRepository.SaveChangesAsync();
        return Result.Ok(new RegisterTimeSheetResultModel(latestTimesheet.Id, info.WorkerFullName, true));
    }

    public async Task<IEnumerable<CompanyProfileJobPositionRateModel>> GetJobPositions(Guid companyId, DateTime startDate, DateTime endDate)
    {
        var result = await timeSheetRepository.GetJobPositions(companyId, startDate, endDate);
        return result;
    }

    public async Task<Result<ClockType>> GetClockType(Guid requestId, DateTime? date)
    {
        if (!date.HasValue)
        {
            return Result.Ok(ClockType.None);
        }
        var workerId = identityServerService.GetUserId();
        var now = timeService.GetCurrentDateTime();
        var timeSheet = await timeSheetRepository.GetLatestTimesheet(workerId, requestId, date.Value);
        if (date.Value.Date != now.Date && timeSheet == null)
        {
            return Result.Ok(ClockType.None);
        }
        else if (timeSheet == null)
        {
            return Result.Ok(ClockType.ClockIn);
        }
        else if (timeSheet.ClockIn.HasValue && timeSheet.ClockOut.HasValue)
        {
            return Result.Ok(ClockType.None);
        }
        var totalHours = now - timeSheet.ClockIn.GetValueOrDefault();
        if (totalHours.Hours <= TimeLimits.DefaultTimeLimits.MaximumHoursDay)
        {
            return Result.Ok(ClockType.ClockOut);
        }
        else
        {
            return Result.Ok(ClockType.None);
        }
    }

    public async Task<Result> RemoveTimeSheet(Guid id)
    {
        var timesheet = await timeSheetRepository.GetTimeSheet(id);
        if (timesheet != null)
        {
            var timesheetUsedByAccounting = await timeSheetRepository.TimesheetUsedByAccounting(id);
            if (!timesheetUsedByAccounting)
            {
                timeSheetRepository.Delete(timesheet);
                await timeSheetRepository.SaveChangesAsync();
                return Result.Ok();
            }
            return Result.Fail("Timesheet currently used in an invoice or paystub please, first delete the invoice.");
        }
        return Result.Fail("Timesheet doesn't exist");
    }

    public async Task<ResultGenerateDocument<MemoryStream>> GetRequestTimesheetFile(Guid requestId)
    {
        var data = await timeSheetRepository.GetRequestTimeSheet(requestId);
        var request = await mediator.Send(new GenerateRequestTimeSheetReport(data.ToList()));
        return request;
    }
}
