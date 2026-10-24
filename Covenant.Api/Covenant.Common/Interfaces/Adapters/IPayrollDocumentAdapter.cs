using Covenant.Common.Models.Accounting.PayStubs;

namespace Covenant.Common.Interfaces.Adapters;

public interface IPayrollDocumentAdapter
{
    PayrollViewModel MapToPayrollViewModel(PayStubDetailModel model);

    PayrollEmailViewModel MapToPayrollEmailViewModel(PayStubDetailModel model);
}
