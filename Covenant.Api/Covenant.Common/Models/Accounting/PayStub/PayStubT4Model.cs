namespace Covenant.Common.Models.Accounting.PayStub
{
    public class PayStubT4Model
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string SecondLastName { get; set; }
        public string Sin { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ProvinceCode { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public IEnumerable<PayStubT4Base> Items { get; set; }
        public string WorkerFullName => $"{FirstName}{(string.IsNullOrWhiteSpace(MiddleName) ? string.Empty : $" {MiddleName}")} {LastName}{(string.IsNullOrWhiteSpace(SecondLastName) ? string.Empty : $" {SecondLastName}")}";
        public string Location => $"{Address} {City}, {ProvinceCode} {PostalCode}";
    }

    public class PayStubT4Tax
    {
        public decimal Cpp { get; set; }
        public decimal EI { get; set; }
        public decimal FederalTax { get; set; }
        public decimal ProvincialTax { get; set; }
        public decimal OtherDeductions { get; set; }
    }

    public class PayStubT4Base
    {
        public string PayStubNumber { get; set; }
        public DateTime DatePaid { get; set; }
        public decimal TotalEarnings { get; set; }
        public PayStubT4Tax Employer { get; set; }
        public PayStubT4Tax Employee { get; set; }
    }
}
