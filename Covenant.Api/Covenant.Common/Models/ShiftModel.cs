using Covenant.Common.Entities;

namespace Covenant.Common.Models
{
    public class ShiftModel
    {
        public bool? Sunday { get; set; }
        public bool? Monday { get; set; }
        public bool? Tuesday { get; set; }
        public bool? Wednesday { get; set; }
        public bool? Thursday { get; set; }
        public bool? Friday { get; set; }
        public bool? Saturday { get; set; }
        public TimeSpan? SundayStart { get; set; }
        public TimeSpan? SundayFinish { get; set; }
        public TimeSpan? MondayStart { get; set; }
        public TimeSpan? MondayFinish { get; set; }
        public TimeSpan? TuesdayStart { get; set; }
        public TimeSpan? TuesdayFinish { get; set; }
        public TimeSpan? WednesdayStart { get; set; }
        public TimeSpan? WednesdayFinish { get; set; }
        public TimeSpan? ThursdayStart { get; set; }
        public TimeSpan? ThursdayFinish { get; set; }
        public TimeSpan? FridayStart { get; set; }
        public TimeSpan? FridayFinish { get; set; }
        public TimeSpan? SaturdayStart { get; set; }
        public TimeSpan? SaturdayFinish { get; set; }
        public string Comments { get; set; }

        public Shift ToShift()
        {
            var shift = new Shift();
            if (Sunday.GetValueOrDefault()) shift.AddSunday(SundayStart.GetValueOrDefault(), SundayFinish.GetValueOrDefault());
            if (Monday.GetValueOrDefault()) shift.AddMonday(MondayStart.GetValueOrDefault(), MondayFinish.GetValueOrDefault());
            if (Tuesday.GetValueOrDefault()) shift.AddTuesday(TuesdayStart.GetValueOrDefault(), TuesdayFinish.GetValueOrDefault());
            if (Wednesday.GetValueOrDefault()) shift.AddWednesday(WednesdayStart.GetValueOrDefault(), WednesdayFinish.GetValueOrDefault());
            if (Thursday.GetValueOrDefault()) shift.AddThursday(ThursdayStart.GetValueOrDefault(), ThursdayFinish.GetValueOrDefault());
            if (Friday.GetValueOrDefault()) shift.AddFriday(FridayStart.GetValueOrDefault(), FridayFinish.GetValueOrDefault());
            if (Saturday.GetValueOrDefault()) shift.AddSaturday(SaturdayStart.GetValueOrDefault(), SaturdayFinish.GetValueOrDefault());
            shift.Comments = Comments;
            return shift;
        }
    }
}