namespace Covenant.Common.Models.Request.TimeSheet
{
    public class TimeSheetHistoryModel
    {
        public int RowNumber { get; set; }
        public int NumberId { get; set; }
        public string BusinessName { get; set; }
        public string JobTitle { get; set; }
        public DateTime Date { get; set; }
        public bool IsHoliday { get; set; }
        public double RegularHours { get; set; }
        public double HolidayHours { get; set; }
        public double OvertimeHours { get; set; }
        public double MissingHours { get; set; }
        public double MissingHoursOvertime { get; set; }
        public double TotalHours => RegularHours + HolidayHours + OvertimeHours + MissingHours + MissingHoursOvertime;
    }
}