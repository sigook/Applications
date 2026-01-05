namespace Covenant.Common.Models.Accounting
{
    public class HoursWorkedResponse
    {
        public string WorkerName { get; set; }
        public string JobPosition { get; set; }
        public decimal PayRate { get; set; }
        public decimal BillRate { get; set; }
        public double RegularHoursWorked { get; set; }
        public double OvertimeHoursWorked { get; set; }
        public double HolidayHoursWorked { get; set; }
        public double NightHoursWorked { get; set; }
        public decimal TotalPayRegularRate { get; set; }
        public decimal TotalPayOvertimeRate { get; set; }
        public decimal TotalPayHolidayRate { get; set; }
        public decimal TotalPayNightRate { get; set; }
        public double TotalHoursWorked => RegularHoursWorked + OvertimeHoursWorked + HolidayHoursWorked + NightHoursWorked;
        public decimal TotalPayRate => TotalPayRegularRate + TotalPayOvertimeRate + TotalPayHolidayRate + TotalPayNightRate;
    }
}
