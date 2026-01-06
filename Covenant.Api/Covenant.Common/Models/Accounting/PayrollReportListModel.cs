namespace Covenant.Common.Models.Accounting
{
    public class PayrollReportListModel
    {
        public Guid Id { get; set; }
        public long NumberId { get; set; }
        public string WorkerFullName { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalPaid { get; set; }
        public string AgencyFullName { get; set; }
        public string CompanyFullName { get; set; }
    }
}
