using Covenant.Common.Configuration;
using Covenant.Common.Utils;
using Covenant.Common.Utils.Extensions;

namespace Covenant.TimeSheetTotal.Models
{
    public readonly struct TotalDailyPrice
    {
        public TotalDailyPrice(Rates rates, decimal agencyRate,
            decimal missingRateAgency, TimeSpan missingHours, TimeSpan missingHoursOvertime,
            TimeSpan regularHours, TimeSpan otherRegularHours, TimeSpan nightShiftHours, TimeSpan holidayHours, TimeSpan overtimeHours)
        {
            if (rates is null) throw new ArgumentNullException(nameof(rates));
            AgencyRate = agencyRate;
            MissingRateAgency = missingRateAgency;
            MissingHours = missingHours;
            MissingHoursOvertime = missingHoursOvertime;
            RegularHours = regularHours;
            OtherRegularHours = otherRegularHours;
            NightShiftHours = nightShiftHours;
            HolidayHours = holidayHours;
            OvertimeHours = overtimeHours;

            MissingRateAgency = MissingRateAgency <= decimal.Zero ? agencyRate : MissingRateAgency;
            Regular = InvoiceFormulas.Regular(agencyRate, RegularHours.TotalHours);
            OtherRegular = InvoiceFormulas.Regular(agencyRate, OtherRegularHours.TotalHours);
            Missing = InvoiceFormulas.Missing(MissingRateAgency, MissingHours.TotalHours);
            MissingOvertime = InvoiceFormulas.Overtime(MissingRateAgency, rates.OverTime, MissingHoursOvertime.TotalHours);
            NightShift = InvoiceFormulas.NightShift(agencyRate, rates.NightShift, NightShiftHours.TotalHours);
            Holiday = InvoiceFormulas.Holiday(agencyRate, rates.Holiday, HolidayHours.TotalHours);
            Overtime = InvoiceFormulas.Overtime(agencyRate, rates.OverTime, OvertimeHours.TotalHours);
            TotalGross = InvoiceFormulas.TotalGross(Regular.Add(OtherRegular), Missing, MissingOvertime, NightShift, Holiday, Overtime);
            TotalNet = TotalGross;
        }

        public decimal AgencyRate { get; }
        public decimal MissingRateAgency { get; }
        public TimeSpan MissingHours { get; }
        public TimeSpan MissingHoursOvertime { get; }
        public TimeSpan RegularHours { get; }
        public TimeSpan OtherRegularHours { get; }
        public TimeSpan NightShiftHours { get; }
        public TimeSpan HolidayHours { get; }
        public TimeSpan OvertimeHours { get; }
        public decimal Regular { get; }
        public decimal OtherRegular { get; }
        public decimal Missing { get; }
        public decimal MissingOvertime { get; }
        public decimal NightShift { get; }
        public decimal Holiday { get; }
        public decimal Overtime { get; }
        public decimal TotalGross { get; }
        public decimal TotalNet { get; }
    }
}