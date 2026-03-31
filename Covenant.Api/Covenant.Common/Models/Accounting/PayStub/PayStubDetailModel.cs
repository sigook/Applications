using Covenant.Common.Enums;

namespace Covenant.Common.Models.Accounting.PayStub
{
    public class PayStubDetailModel
    {
        public Guid Id { get; set; }
        public long NumberId { get; set; }
        public long PayrollNumberId { get; set; }
        public string AgencyFullName { get; set; }
        public string AgencyPhone { get; set; }
        public int? AgencyPhoneExt { get; set; }
        public string AgencyLogoFileName { get; set; }
        public string AgencyLocation { get; set; }
        public string WorkerFullName { get; set; }
        public string WorkerEmail { get; set; }
        public string Position { get; set; }
        public string SinNumber { get; set; }
        public int EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Vacations { get; set; }
        public decimal Earnings { get; set; }
        public decimal DeductionCpp { get; set; }
        public decimal DeductionEi { get; set; }
        public decimal DeductionTax { get; set; }
        public decimal DeductionTotal { get; set; }
        public decimal Gross { get; set; }
        public decimal TotalNet { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal DeductionOthers { get; set; }
        public string PayrollNumber { get; set; }
        public decimal DeductionProvincialTax { get; set; }
        public List<PayStubDetailItemModel> Items { get; set; } = new List<PayStubDetailItemModel>();
        public List<PayStubDetailItemModel> OtherDeductions { get; set; } = new List<PayStubDetailItemModel>();
        public TaxCategory FederalCategory { get; set; }
        public TaxCategory ProvincialCategory { get; set; }
        public Guid WorkerProfileId { get; set; }
        public PayStubYtdModel Ytd { get; set; }
    }
}