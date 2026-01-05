using Covenant.Common.Models.Accounting.PayStub;

namespace Covenant.PayStubs.Models
{
    public class WeeklyPayrollUSAModel
    {
        public string PayStubNumber { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public decimal GrossPayment { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal TotalPaid { get; set; }
        public DateTime WeedEnding { get; set; }
        public DateTime PaymentDate { get; set; }
        public List<WeeklyPayStubItemModel> Compensation { get; set; } = new List<WeeklyPayStubItemModel>();
        public List<WeeklyPayStubItemModel> Taxes { get; set; } = new List<WeeklyPayStubItemModel>();
        public List<WeeklyPayStubItemModel> Deductions { get; set; } = new List<WeeklyPayStubItemModel>();
        public List<string> Companies { get; set; } = new List<string>();
    }
}