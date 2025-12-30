using System;

namespace Covenant.TimeSheetTotal.Models
{
	public readonly struct TotalizatorCreateInvoiceParams
	{
		public TotalizatorCreateInvoiceParams(
			Guid requestId,
			DateTime date,
			DateTime timeInApproved,
			DateTime timeOutApproved,
			TimeSpan missingHours,
			TimeSpan missingHoursOvertime,
			decimal agencyRate,
			decimal missingRateAgency,
			bool isHoliday,
			bool holidayIsPaid,
			bool breakIsPaid,
			TimeSpan durationBreak,
			Guid timeSheetId,
			string jobTitle)
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
			BreakIsPaid = breakIsPaid;
			DurationBreak = durationBreak;
			HolidayIsPaid = holidayIsPaid;
			TimeSheetId = timeSheetId;
			JobTitle = jobTitle;
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
		public bool BreakIsPaid { get; }
		public TimeSpan DurationBreak { get; }
		public bool HolidayIsPaid { get; }
		public Guid TimeSheetId { get; }
		public string JobTitle { get; }
	}
}