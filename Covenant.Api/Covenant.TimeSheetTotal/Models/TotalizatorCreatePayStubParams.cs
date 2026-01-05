using System;

namespace Covenant.TimeSheetTotal.Models
{
	public readonly struct TotalizatorCreatePayStubParams
	{
		public TotalizatorCreatePayStubParams(
			Guid requestId,
			DateTime date,
			DateTime timeInApproved,
			DateTime timeOutApproved,
			TimeSpan missingHours,
			TimeSpan missingHoursOvertime,
			decimal workerRate,
			decimal missingRateWorker,
			bool isHoliday,
			bool holidayIsPaid,
			bool breakIsPaid,
			TimeSpan durationBreak,
			Guid timeSheetId)
		{
			RequestId = requestId;
			Date = date;
			IsHoliday = isHoliday;
			TimeOutApproved = timeOutApproved;
			TimeInApproved = timeInApproved;
			MissingHoursOvertime = missingHoursOvertime;
			MissingHours = missingHours;
			WorkerRate = workerRate;
			MissingRateWorker = missingRateWorker;
			BreakIsPaid = breakIsPaid;
			DurationBreak = durationBreak;
			HolidayIsPaid = holidayIsPaid;
			TimeSheetId = timeSheetId;
		}
		public Guid RequestId { get; }
		public DateTime Date { get; }
		public bool IsHoliday { get; }
		public DateTime TimeOutApproved { get; }
		public DateTime TimeInApproved { get; }
		public TimeSpan MissingHoursOvertime { get; }
		public TimeSpan MissingHours { get; }
		public decimal WorkerRate { get; }
		public decimal MissingRateWorker { get; }
		public bool BreakIsPaid { get; }
		public TimeSpan DurationBreak { get; }
		public bool HolidayIsPaid { get; }
		public Guid TimeSheetId { get; }
	}
}