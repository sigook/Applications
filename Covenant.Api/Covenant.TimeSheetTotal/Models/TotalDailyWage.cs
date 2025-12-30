using Covenant.Common.Configuration;
using Covenant.Common.Utils;
using Covenant.Common.Utils.Extensions;

namespace Covenant.TimeSheetTotal.Models
{
    public readonly struct TotalDailyWage
    {
        public decimal WorkerRate { get; }
        public decimal HolidayRate { get; }
        public decimal OvertimeRate { get; }
        public decimal NightShiftRate { get; }
        public decimal MissingRateWorker { get; }
        public decimal MissingOvertimeRate { get; }
        public TimeSpan MissingHours { get; }
        public TimeSpan MissingOvertimeHours { get; }
        public TimeSpan RegularHours { get; }
        public TimeSpan OtherRegularHours { get; }
        public TimeSpan NightShiftHours { get; }
        public TimeSpan HolidayHours { get; }
        public TimeSpan OvertimeHours { get; }

        public TotalDailyWage(Rates rates, decimal workerRate, decimal missingRateWorker,
            TimeSpan missingHours, TimeSpan missingOvertimeHours, TimeSpan regularHours, TimeSpan otherRegularHours,
            TimeSpan nightShiftHours, TimeSpan holidayHours, TimeSpan overtimeHours)
        {
            if (rates is null) throw new ArgumentNullException(nameof(rates));
            WorkerRate = workerRate;
            MissingRateWorker = missingRateWorker <= decimal.Zero ? workerRate : missingRateWorker;
            MissingHours = missingHours;
            MissingOvertimeHours = missingOvertimeHours;
            RegularHours = regularHours;
            OtherRegularHours = otherRegularHours;
            NightShiftHours = nightShiftHours;
            HolidayHours = holidayHours;
            OvertimeHours = overtimeHours;
            HolidayRate = rates.Holiday * WorkerRate;
            OvertimeRate = rates.OverTime * WorkerRate;
            NightShiftRate = rates.NightShift * WorkerRate;
            MissingOvertimeRate = rates.OverTime * MissingRateWorker;

            Regular = PayrollFormulas.Regular(WorkerRate, RegularHours.TotalHours).DefaultMoneyRound();//Include for public holidays
            OtherRegular = PayrollFormulas.Regular(WorkerRate, OtherRegularHours.TotalHours).DefaultMoneyRound();//Include for public holidays
            Overtime = PayrollFormulas.Overtime(WorkerRate, rates.OverTime, OvertimeHours.TotalHours).DefaultMoneyRound();//It isn't include for public holidays
            Holiday = PayrollFormulas.Holiday(WorkerRate, rates.Holiday, HolidayHours.TotalHours).DefaultMoneyRound();//Include for public holidays
            NightShift = PayrollFormulas.NightShift(WorkerRate, rates.NightShift, NightShiftHours.TotalHours).DefaultMoneyRound();//Include for public holidays
            Missing = PayrollFormulas.Missing(MissingRateWorker, MissingHours.TotalHours).DefaultMoneyRound();//Include for public holidays
            MissingOvertime = PayrollFormulas.Overtime(MissingRateWorker, rates.OverTime, MissingOvertimeHours.TotalHours).DefaultMoneyRound();//It isn't include for public holidays
            TotalGross = Regular.Add(OtherRegular).Add(Missing).Add(MissingOvertime).Add(NightShift).Add(Holiday).Add(Overtime).DefaultMoneyRound();
        }
        public decimal Regular { get; }
        public decimal OtherRegular { get; }
        public decimal Overtime { get; }
        public decimal Holiday { get; }
        public decimal NightShift { get; }
        public decimal Missing { get; }
        public decimal MissingOvertime { get; }
        public decimal TotalGross { get; }
    }
}