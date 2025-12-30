using System;

namespace Covenant.TimeSheetTotal.Models
{
	public readonly struct TotalizatorParams
	{
		public TotalizatorParams(
			Guid requestId,
			DateTime date,
			DateTime timeInApproved,
			DateTime timeOutApproved,
			TimeSpan missingHours,
			TimeSpan missingHoursOvertime,
			decimal agencyRate,
			decimal missingRateAgency,
			decimal workerRate,
			decimal missingRateWorker,
			bool isHoliday,
			bool holidayIsPaid,
			bool breakIsPaid,
			TimeSpan durationBreak,
			bool payNightShift,
			TimeSpan? timeStartNightShift,
			TimeSpan? timeEndNightShift,
			Guid timeSheetId)
		{
			RequestId = requestId;
			Date = date;
			IsHoliday = isHoliday;
			TimeOutApproved = timeOutApproved;
			TimeInApproved = timeInApproved;
			AgencyRate = agencyRate;
			MissingRateAgency = missingRateAgency;
			MissingHoursOvertime = missingHoursOvertime;
			MissingHours = missingHours;
			WorkerRate = workerRate;
			MissingRateWorker = missingRateWorker;
			BreakIsPaid = breakIsPaid;
			DurationBreak = durationBreak;
			HolidayIsPaid = holidayIsPaid;
			PayNightShift = payNightShift;
			TimeStartNightShift = timeStartNightShift;
			TimeEndNightShift = timeEndNightShift;
			TimeSheetId = timeSheetId;
		}
		public Guid RequestId { get; }
		public DateTime Date { get; }
		public bool IsHoliday { get; }
		public DateTime TimeOutApproved { get; }
		public DateTime TimeInApproved { get; }
		public decimal AgencyRate { get; }
		public decimal MissingRateAgency { get; }
		public TimeSpan MissingHoursOvertime { get; }
		public TimeSpan MissingHours { get; }
		public decimal WorkerRate { get; }
		public decimal MissingRateWorker { get; }
		public bool BreakIsPaid { get; }
		public TimeSpan DurationBreak { get; }
		public bool HolidayIsPaid { get; }
		public bool PayNightShift { get; }
		public TimeSpan? TimeStartNightShift { get; }
		public TimeSpan? TimeEndNightShift { get; }
		public Guid TimeSheetId { get; }
	}
}