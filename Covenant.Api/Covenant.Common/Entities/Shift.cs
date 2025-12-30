using Covenant.Common.Functionals;
using Covenant.Common.Resources;
using System.Text;

namespace Covenant.Common.Entities
{
    public class Shift
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public bool? Sunday { get; private set; }
        public bool? Monday { get; private set; }
        public bool? Tuesday { get; private set; }
        public bool? Wednesday { get; private set; }
        public bool? Thursday { get; private set; }
        public bool? Friday { get; private set; }
        public bool? Saturday { get; private set; }

        public TimeSpan? SundayStart { get; private set; }
        public TimeSpan? SundayFinish { get; private set; }

        public TimeSpan? MondayStart { get; private set; }
        public TimeSpan? MondayFinish { get; private set; }

        public TimeSpan? TuesdayStart { get; private set; }
        public TimeSpan? TuesdayFinish { get; private set; }

        public TimeSpan? WednesdayStart { get; private set; }
        public TimeSpan? WednesdayFinish { get; private set; }

        public TimeSpan? ThursdayStart { get; private set; }
        public TimeSpan? ThursdayFinish { get; private set; }

        public TimeSpan? FridayStart { get; private set; }
        public TimeSpan? FridayFinish { get; private set; }

        public TimeSpan? SaturdayStart { get; private set; }
        public TimeSpan? SaturdayFinish { get; private set; }

        public string Comments { get; set; }

        public string DisplayShift { get; private set; }

        public Result AddSunday(TimeSpan start, TimeSpan finish) => AddDay(DayOfWeek.Sunday, start, finish);
        public Result AddMonday(TimeSpan start, TimeSpan finish) => AddDay(DayOfWeek.Monday, start, finish);
        public Result AddTuesday(TimeSpan start, TimeSpan finish) => AddDay(DayOfWeek.Tuesday, start, finish);
        public Result AddWednesday(TimeSpan start, TimeSpan finish) => AddDay(DayOfWeek.Wednesday, start, finish);
        public Result AddThursday(TimeSpan start, TimeSpan finish) => AddDay(DayOfWeek.Thursday, start, finish);
        public Result AddFriday(TimeSpan start, TimeSpan finish) => AddDay(DayOfWeek.Friday, start, finish);
        public Result AddSaturday(TimeSpan start, TimeSpan finish) => AddDay(DayOfWeek.Saturday, start, finish);

        public Result AddDay(DayOfWeek day, TimeSpan start, TimeSpan finish)
        {
            if (start < TimeSpan.Zero) return Result.Fail(ValidationMessages.GreaterThanOrEqualMsg("Start", "00:00"));
            if (start.TotalHours >= 24) return Result.Fail(ValidationMessages.LessThanMsg("Start", "23:59"));

            if (finish < TimeSpan.Zero) return Result.Fail(ValidationMessages.GreaterThanOrEqualMsg("Finish", "00:00"));
            if (finish.TotalHours >= 24) return Result.Fail(ValidationMessages.LessThanMsg("Finish", "23:59"));

            switch (day)
            {
                case DayOfWeek.Sunday:
                    Sunday = true;
                    SundayStart = start;
                    SundayFinish = finish;
                    break;
                case DayOfWeek.Monday:
                    Monday = true;
                    MondayStart = start;
                    MondayFinish = finish;
                    break;
                case DayOfWeek.Tuesday:
                    Tuesday = true;
                    TuesdayStart = start;
                    TuesdayFinish = finish;
                    break;
                case DayOfWeek.Wednesday:
                    Wednesday = true;
                    WednesdayStart = start;
                    WednesdayFinish = finish;
                    break;
                case DayOfWeek.Thursday:
                    Thursday = true;
                    ThursdayStart = start;
                    ThursdayFinish = finish;
                    break;
                case DayOfWeek.Friday:
                    Friday = true;
                    FridayStart = start;
                    FridayFinish = finish;
                    break;
                case DayOfWeek.Saturday:
                    Saturday = true;
                    SaturdayStart = start;
                    SaturdayFinish = finish;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(day), day, null);
            }

            UpdateDisplayShift();
            return Result.Ok();
        }

        public void Update(Shift newShift)
        {
            if (newShift is null) throw new ArgumentNullException(nameof(newShift));
            Sunday = newShift.Sunday;
            SundayStart = newShift.SundayStart;
            SundayFinish = newShift.SundayFinish;
            Monday = newShift.Monday;
            MondayStart = newShift.MondayStart;
            MondayFinish = newShift.MondayFinish;
            Tuesday = newShift.Tuesday;
            TuesdayStart = newShift.TuesdayStart;
            TuesdayFinish = newShift.TuesdayFinish;
            Wednesday = newShift.Wednesday;
            WednesdayStart = newShift.WednesdayStart;
            WednesdayFinish = newShift.WednesdayFinish;
            Thursday = newShift.Thursday;
            ThursdayStart = newShift.ThursdayStart;
            ThursdayFinish = newShift.ThursdayFinish;
            Friday = newShift.Friday;
            FridayStart = newShift.FridayStart;
            FridayFinish = newShift.FridayFinish;
            Saturday = newShift.Saturday;
            SaturdayStart = newShift.SaturdayStart;
            SaturdayFinish = newShift.SaturdayFinish;
            Comments = newShift.Comments;
            UpdateDisplayShift();
        }

        private void UpdateDisplayShift()
        {
            try
            {
                var newDisplay = new StringBuilder();
                if (IsMondayToFriday) newDisplay.Append("Mon-Fri");
                else if (IsSundayToSaturday) newDisplay.Append("Sun-Sat");
                else if (IsMondayWednesdayAndFriday) newDisplay.Append("Mon-Wed-Fri");
                else if (IsMondayToSaturday) newDisplay.Append("Mon-Sat");
                else
                {
                    newDisplay.Append(string.Join("-", new[]
                    {
                    Sunday.GetValueOrDefault() ? "Sun" : null,
                    Monday.GetValueOrDefault() ? "Mon" : null,
                    Tuesday.GetValueOrDefault() ? "Tue" : null,
                    Wednesday.GetValueOrDefault() ? "Wed" : null,
                    Thursday.GetValueOrDefault() ? "Thu" : null,
                    Friday.GetValueOrDefault() ? "Fri" : null,
                    Saturday.GetValueOrDefault() ? "Sat" : null,
                }.Where(w => !string.IsNullOrEmpty(w))));
                }

                List<TimeSpan> start = new[] { SundayStart, MondayStart, TuesdayStart, WednesdayStart, ThursdayStart, FridayStart, SaturdayStart }
                    .Where(w => w.HasValue).GroupBy(g => g.GetValueOrDefault())
                    .Select(sel => sel.Key).ToList();
                if (start.Count == 1)
                {
                    List<TimeSpan> finish = new[] { SaturdayFinish, MondayFinish, TuesdayFinish, WednesdayFinish, ThursdayFinish, FridayFinish, SaturdayFinish }
                        .Where(w => w.HasValue).GroupBy(g => g.GetValueOrDefault())
                        .Select(sel => sel.Key).ToList();
                    if (finish.Count == 1)
                    {
                        newDisplay.Append(" ");
                        newDisplay.Append(start.First().ToString(@"hh\:mm"));
                        newDisplay.Append(" to ");
                        newDisplay.Append(finish.First().ToString(@"hh\:mm"));
                    }
                }

                DisplayShift = newDisplay.ToString();
            }
            catch (Exception)
            {
                DisplayShift = string.Empty;
            }
        }

        private bool IsMondayToFriday => Monday.GetValueOrDefault() && Tuesday.GetValueOrDefault()
                                                                    && Wednesday.GetValueOrDefault()
                                                                    && Thursday.GetValueOrDefault()
                                                                    && Friday.GetValueOrDefault()
                                                                    && !Saturday.GetValueOrDefault()
                                                                    && !Sunday.GetValueOrDefault();

        private bool IsMondayToSaturday => Monday.GetValueOrDefault() && Tuesday.GetValueOrDefault()
                                                                      && Wednesday.GetValueOrDefault()
                                                                      && Thursday.GetValueOrDefault()
                                                                      && Friday.GetValueOrDefault()
                                                                      && Saturday.GetValueOrDefault()
                                                                      && !Sunday.GetValueOrDefault();

        private bool IsSundayToSaturday => Monday.GetValueOrDefault() && Tuesday.GetValueOrDefault()
                                                                      && Wednesday.GetValueOrDefault()
                                                                      && Thursday.GetValueOrDefault()
                                                                      && Friday.GetValueOrDefault()
                                                                      && Saturday.GetValueOrDefault()
                                                                      && Sunday.GetValueOrDefault();

        private bool IsMondayWednesdayAndFriday => Monday.GetValueOrDefault() && !Tuesday.GetValueOrDefault()
                                                                              && Wednesday.GetValueOrDefault()
                                                                              && !Thursday.GetValueOrDefault()
                                                                              && Friday.GetValueOrDefault()
                                                                              && !Saturday.GetValueOrDefault()
                                                                              && !Sunday.GetValueOrDefault();
    }
}