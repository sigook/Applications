using Covenant.Common.Utils.Extensions;

namespace Covenant.Common.Utils
{
    public static class InvoiceFormulas
    {
        public static decimal Regular(decimal rate, double regularHours) => decimal.Multiply(rate, (decimal)regularHours);

        public static decimal Missing(decimal rate, double missingHours) => decimal.Multiply(rate, (decimal)missingHours);

        public static decimal NightShift(decimal rate, decimal nightShiftRate, double nightShiftHours) =>
            decimal.Multiply(decimal.Multiply(rate, nightShiftRate), (decimal)nightShiftHours);

        public static decimal Holiday(decimal rate, decimal holidayRate, double holidayHours) =>
            decimal.Multiply(decimal.Multiply(rate, holidayRate), (decimal)holidayHours);

        public static decimal Overtime(decimal rate, decimal overTimeRate, double overtimeHours) =>
            decimal.Multiply(decimal.Multiply(rate, overTimeRate), (decimal)overtimeHours);

        public static decimal TotalGross(decimal regular, decimal missing, decimal missingOvertime, decimal nightShift, decimal holiday, decimal overtime) =>
            regular.Add(missing).Add(missingOvertime).Add(nightShift).Add(holiday).Add(overtime);
    }
}