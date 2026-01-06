namespace Covenant.Common.Configuration
{
    public class TimeLimits
    {
        public static readonly TimeLimits DefaultTimeLimits = new TimeLimits(TimeSpan.FromHours(44), 14);

        public TimeLimits()
        { 
        }

        public TimeLimits(TimeSpan maxHoursWeek, double maximumHoursDay)
        {
            MaxHoursWeek = maxHoursWeek;
            MaximumHoursDay = maximumHoursDay;
        }

        public TimeSpan MaxHoursWeek { get; set; }
        public double MaximumHoursDay { get; set; } = 12;
    }
}