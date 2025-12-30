namespace Covenant.Common.Models.Accounting.Invoice
{
    public class InvoiceItemTotalModel
    {
        public Guid RequestId { get; set; }
        public string JobTitle { get; set; }
        public decimal Regular { get; set; }
        public decimal OtherRegular { get; set; }
        public decimal Missing { get; set; }
        public decimal MissingOvertime { get; set; }
        public decimal NightShift { get; set; }
        public decimal Holiday { get; set; }
        public decimal Overtime { get; set; }
        public double RegularHours { get; set; }
        public double OtherRegularHours { get; set; }
        public double MissingHours { get; set; }
        public double MissingHoursOvertime { get; set; }
        public double NightShiftHours { get; set; }
        public double HolidayHours { get; set; }
        public double OvertimeHours { get; set; }
        public decimal MissingRateAgency { get; set; }
    }
}