using System;
using System.Threading.Tasks;
using Covenant.Common.Args;

namespace Covenant.CreateInvoiceCharge.Services
{
    public interface ICreateInvoiceChargeService
    {
        event Func<object,InvoiceChargeSucceededEventArgs, Task> OnInvoiceChargeSucceeded;
        event Func<object,InvoiceChargeFailedEventArgs, Task> OnInvoiceChargeFailed;
        Task CreatePendingInvoiceCharge();
    }
}