namespace Covenant.Common.Models.Accounting.PayStub
{
    public class PayStubListModel
    {
        public Guid Id { get; set; }
        public long NumberId { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalPaid { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string SecondLastName { get; set; }
        public string WorkerFullName { get; set; }
        public string PayStubNumber { get; set; }
        public long PayStubNumberId { get; set; }
        public bool CanDelete { get; set; }
    }
}