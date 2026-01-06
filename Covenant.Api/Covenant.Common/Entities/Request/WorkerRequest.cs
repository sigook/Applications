using Covenant.Common.Enums;
using Covenant.Common.Functionals;

namespace Covenant.Common.Entities.Request
{
    public class WorkerRequest
    {
        private WorkerRequest()
        {
        }

        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid WorkerId { get; private set; }
        public User Worker { get; private set; }

        public Guid RequestId { get; private set; }
        public Request Request { get; set; }

        public WorkerRequestStatus WorkerRequestStatus { get; private set; }
        public ICollection<TimeSheet> TimeSheets { get; private set; } = new List<TimeSheet>();
        public IEnumerable<WorkerRequestNote> Notes { get; private set; } = new List<WorkerRequestNote>();

        public DateTime? StartWorking { get; private set; }
        public DateTime? WeekStartWorking { get; private set; }

        public Result UpdateStartWorking(DateTime startWorking)
        {
            if (TimeSheets.Any()) return Result.Fail("Start working cannot be changed");
            if (startWorking == default) startWorking = CreatedAt;
            StartWorking = startWorking;
            WeekStartWorking = GetWeekStartWorking(startWorking);
            return Result.Ok();
        }

        public string CreatedBy { get; private set; }
        public DateTime CreatedAt { get; internal set; } = DateTime.Now;
        public string RejectedBy { get; private set; }
        public DateTime? RejectedAt { get; private set; }

        public string RejectComments { get; private set; }

        public bool IsRejected => WorkerRequestStatus == WorkerRequestStatus.Rejected;
        public bool IsBooked => WorkerRequestStatus == WorkerRequestStatus.Booked;

        public DateTime? LimitDateToAddTimeSheet => RejectedAt?.AddMonths(1);

        public Result Book()
        {
            WorkerRequestStatus = WorkerRequestStatus.Booked;
            RejectComments = null;
            RejectedAt = null;
            return Result.Ok();
        }

        public Result Reject(string comments = null, DateTime? rejectedAt = null, string rejectedBy = null)
        {
            if (IsRejected) return Result.Ok();
            WorkerRequestStatus = WorkerRequestStatus.Rejected;
            RejectComments = comments;
            RejectedAt = rejectedAt ?? DateTime.Now;
            RejectedBy = rejectedBy;
            return Result.Ok();
        }

        public bool ContainsTimeSheet(TimeSheet timeSheet) => TimeSheets.Any(a => a.Date.Date == timeSheet.Date.Date);

        public Result AddTimeSheet(TimeSheet timeSheet)
        {
            if (timeSheet is null) throw new ArgumentNullException(nameof(timeSheet));
            if (ContainsTimeSheet(timeSheet)) return Result.Fail($"TimeSheet for the date {timeSheet.Date:D} was already created");
            TimeSheets.Add(timeSheet);
            return Result.Ok();
        }

        private static DateTime GetWeekStartWorking(DateTime startWorking)
        {
            switch (startWorking.DayOfWeek)
            {
                case DayOfWeek.Sunday: return startWorking.AddDays(0);
                case DayOfWeek.Monday: return startWorking.AddDays(-1);
                case DayOfWeek.Tuesday: return startWorking.AddDays(-2);
                case DayOfWeek.Wednesday: return startWorking.AddDays(-3);
                case DayOfWeek.Thursday: return startWorking.AddDays(-4);
                case DayOfWeek.Friday: return startWorking.AddDays(-5);
                case DayOfWeek.Saturday: return startWorking.AddDays(-6);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public static WorkerRequest AgencyBook(Guid workerId, Guid requestId, string createdBy = default) =>
            new WorkerRequest { WorkerId = workerId, RequestId = requestId, CreatedBy = createdBy, WorkerRequestStatus = WorkerRequestStatus.Booked };
    }
}