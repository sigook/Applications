using System;

namespace Covenant.Common.Args
{
    public class InvoiceChargeFailedEventArgs : EventArgs
    {
        public long NumberId { get; set; }
        public decimal Amount { get; set; }
        public Guid CompanyId { get; set; }
        public string CompanyEmail { get; set; }
        public InvoiceChargeFailedEventArgs(long numberId, decimal amount, Guid companyId, string companyEmail)
        {
            NumberId = numberId;
            Amount = amount;
            CompanyId = companyId;
            CompanyEmail = companyEmail;
        }
    }
}