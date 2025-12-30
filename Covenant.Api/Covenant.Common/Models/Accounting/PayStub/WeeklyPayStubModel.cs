namespace Covenant.Common.Models.Accounting.PayStub
{
    public class WeeklyPayStubModel
    {
        public string PayStubNumber { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public decimal GrossPayment { get; set; }
        public decimal Vacations { get; set; }
        public decimal PublicHoliday { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal Cpp { get; set; }
        public decimal Ei { get; set; }
        public decimal FederalTax { get; set; }
        public decimal ProvincialTax { get; set; }
        public decimal OtherDeductions { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal TotalPaid { get; set; }
        public DateTime WeedEnding { get; set; }
        public DateTime PaymentDate { get; set; }
        public List<WeeklyPayStubItemModel> Items { get; set; } = new List<WeeklyPayStubItemModel>();
        public List<string> Companies { get; set; } = new List<string>();
        public List<string> OtherDeductionsDetail { get; set; } = new List<string>();
    }
}