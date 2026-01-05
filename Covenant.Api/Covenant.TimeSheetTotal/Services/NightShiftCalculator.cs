using System;

namespace Covenant.TimeSheetTotal.Services
{
    public static class NightShiftCalculator
    {
        public static TimeSpan Calculate(TimeSpan timeIn,TimeSpan timeOut,TimeSpan startNightShift, TimeSpan endNightShift)
        {
            if(endNightShift < startNightShift)endNightShift = endNightShift.Add(TimeSpan.FromHours(24));
            if (startNightShift.Ticks < timeOut.Ticks && endNightShift.Ticks > timeIn.Ticks)
            {
                TimeSpan start = TimeSpan.FromTicks(Math.Max(startNightShift.Ticks, timeIn.Ticks));
                TimeSpan end = TimeSpan.FromTicks(Math.Min(endNightShift.Ticks, timeOut.Ticks));
                return end.Subtract(start);
            }
            return TimeSpan.Zero;
        }
    }
}