using Covenant.Common.Configuration;
using Covenant.Common.Functionals;
using Covenant.Common.Resources;

namespace Covenant.Common.Entities.Request
{
    /// <summary>
    /// When TimeOut date is different from TimeIn date, the date must be changed it
    /// to have the same date. The important thing is to preserve the right number of hours.
    /// </summary>
    public class TimeSheet
    {
        public const int TotalOfMinutesToWaitToClockOut = 3;
        public const int TotalOfMinutesToAllowClockOutAgain = 5;
        public const string DefaultComment = "Created and approved by agency";

        private TimeSheet()
        {
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Date { get; set; }
        public bool IsHoliday { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }
        public DateTime? TimeInApproved { get; set; }
        public DateTime? TimeOutApproved { get; set; }
        public string Comment { get; set; }
        public TimeSpan MissingHours { get; set; }
        public TimeSpan MissingHoursOvertime { get; set; }
        public decimal MissingRateWorker { get; set; }
        public decimal MissingRateAgency { get; set; }
        public decimal DeductionsOthers { get; set; }
        public string DeductionsOthersDescription { get; set; }
        public decimal BonusOrOthers { get; set; }
        public string BonusOrOthersDescription { get; set; }
        public decimal Reimbursements { get; set; }
        public string ReimbursementsDescription { get; set; }
        public Guid WorkerRequestId { get; set; }
        public DateTime? ClockIn { get; set; }
        public DateTime? ClockOut { get; set; }
        public DateTime? ClockInRounded { get; set; }
        public DateTime? ClockOutRounded { get; set; }
        public TimeSheetTotal TimeSheetTotal { get; set; }
        public WorkerRequest WorkerRequest { get; set; }
        public double TotalHours => TimeOut.HasValue ? (TimeOut - TimeIn).GetValueOrDefault().TotalHours : 0;
        public double TotalHoursApproved => TimeInApproved.HasValue && TimeOutApproved.HasValue ? (TimeOutApproved - TimeInApproved).GetValueOrDefault().TotalHours : 0;

        public Result AddClockOut(DateTime clockOut, DateTime? clockOutRounded = null)
        {
            if (ClockIn is null) return Result.Fail("Clock in is empty");
            if (clockOut < ClockIn) return Result.Fail("Clock out must be greater than Clock in");

            TimeSpan totalMinutesAfterClockIn = clockOut - ClockIn.GetValueOrDefault();
            if (totalMinutesAfterClockIn < TimeSpan.FromMinutes(TotalOfMinutesToWaitToClockOut))
                return Result.Fail($"Please wait {TotalOfMinutesToWaitToClockOut} minutes to clock out.");

            if (ClockOut.HasValue)
            {
                TimeSpan totalMinutesAfterTimeOut = clockOut - ClockOut.GetValueOrDefault();
                if (totalMinutesAfterTimeOut > TimeSpan.FromMinutes(TotalOfMinutesToAllowClockOutAgain)) return Result.Fail("You already clock out today.");
            }

            DateTime inForTotal;
            DateTime outForTotal;

            if (ClockInRounded.HasValue && clockOutRounded.HasValue)
            {
                inForTotal = ClockInRounded.Value;
                outForTotal = clockOutRounded.Value;
            }
            else
            {
                inForTotal = ClockIn.GetValueOrDefault();
                outForTotal = clockOut;
            }

            double maximumDayHours = new TimeSpan(23, 59, 00).TotalHours;
            double totalHours = (outForTotal - inForTotal).TotalHours;
            if (totalHours > maximumDayHours) totalHours = maximumDayHours;
            TimeIn = inForTotal.Date;//The time for timeIn should be always 00:00:00
            TimeOut = TimeIn.AddHours(totalHours);//Time out should be always in the same date that time In

            ClockOut = clockOut;
            ClockOutRounded = clockOutRounded;
            return Result.Ok();
        }

        public bool IsClockOutValid(DateTime now)
        {
            var totalHours = (now - ClockIn.GetValueOrDefault()).TotalHours;
            return totalHours <= TimeLimits.DefaultTimeLimits.MaximumHoursDay && ClockIn.HasValue && !ClockOut.HasValue;
        }

        public Result AddApprovedTime(DateTime timeInApproved, DateTime timeOutApproved)
        {
            if (timeOutApproved.Date != timeInApproved.Date) return Result.Fail("Time in and Time out must be in the same date");
            if (timeInApproved > timeOutApproved) return Result.Fail("Time in cannot be greater than Time out");
            if (timeInApproved.Date != TimeIn.Date) return Result.Fail($"Time in date must be equal to {TimeIn:D}");
            TimeInApproved = timeInApproved;
            TimeOutApproved = timeOutApproved;
            return Result.Ok();
        }

        public void AddBonusOrOthers(decimal value, string description)
        {
            if (value <= 0) return;
            BonusOrOthers = value;
            BonusOrOthersDescription = description;
        }

        public Result AddDeductionsOthers(decimal deduction, string description)
        {
            if (deduction > 1000) return Result.Fail("Deduction must be between 0 and 1000");
            if (deduction < 0) return Result.Fail("Deduction must be between 0 and 1000");
            DeductionsOthers = deduction;
            DeductionsOthersDescription = description;
            return Result.Ok();
        }

        public void AddReimbursements(decimal value, string description)
        {
            if (value > 0)
            {
                Reimbursements = value;
                ReimbursementsDescription = description;
            }
        }

        public static Result<TimeSheet> CreateTimeSheet(WorkerRequest workerRequest, DateTime date, TimeSpan hours, bool isHoliday = false, string createdBy = null, DateTime? now = null)
        {
            if (workerRequest is null) throw new ArgumentNullException(nameof(workerRequest));
            Result<TimeSheet> result = CreateTimeSheet(workerRequest.Id, date, hours, isHoliday, createdBy, now);
            if (result) result.Value.WorkerRequest = workerRequest;
            return result;
        }

        public static Result<TimeSheet> CreateTimeSheet(Guid workerRequestId, DateTime date, TimeSpan hours, bool isHoliday = false, string createdBy = null, DateTime? now = null)
        {
            if (hours.TotalHours >= 24) return Result.Fail<TimeSheet>("The maximum number of hours is 23:59");
            date = date.Date;
            now = now is null || now.GetValueOrDefault() == default ? DateTime.Now : now;
            DateTime minimum = now.GetValueOrDefault().AddYears(-1);
            if (date < minimum)
                return Result.Fail<TimeSheet>(ValidationMessages.GreaterThan("Date", minimum.ToString("D")));
            DateTime timeIn = date;
            DateTime timeOut = timeIn.AddHours(hours.TotalHours);
            return Result.Ok(new TimeSheet
            {
                WorkerRequestId = workerRequestId,
                Date = date,
                TimeIn = timeIn,
                TimeOut = timeOut,
                IsHoliday = isHoliday,
                TimeInApproved = timeIn,
                TimeOutApproved = timeOut,
                Comment = string.IsNullOrEmpty(createdBy) ? DefaultComment : $"Created and approved by {createdBy}"
            });
        }

        public static Result<TimeSheet> WorkerClockIn(Guid workerRequestId, DateTime clockIn, bool isHoliday = false, DateTime? clockInRounded = null)
        {
            return Result.Ok(new TimeSheet
            {
                WorkerRequestId = workerRequestId,
                Date = clockIn.Date,
                TimeIn = clockIn.Date,
                ClockIn = clockIn,
                ClockInRounded = clockInRounded,
                IsHoliday = isHoliday
            });
        }
    }
}