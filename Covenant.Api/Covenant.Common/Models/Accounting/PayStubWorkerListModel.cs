namespace Covenant.Common.Models.Accounting
{
    public class PayStubWorkerListModel
    {
        public Guid Id { get; set; }
        public long NumberId { get; set; }
        public long PayStubNumberId { get; set; }
        public string PayStubNumber { get; set; }
        public string AgencyFullName { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalPaid { get; set; }
    }
}
