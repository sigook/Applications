using Covenant.Common.Repositories.Agencies;
using Covenant.Core.BL.Interfaces.Accounting.Invoices;

namespace Covenant.Core.BL.Services.Accounting.Invoices;

public class InvoiceCountryStrategyResolver(
    IEnumerable<IInvoiceCountryStrategy> strategies,
    IAgencyRepository agencyRepository) : IInvoiceCountryStrategyResolver
{
    public async Task<IInvoiceCountryStrategy> ResolveForAgency(Guid agencyId)
    {
        var billingLocation = await agencyRepository.GetBillingLocation(agencyId);
        bool isUSA = billingLocation?.IsUSA == true;
        return strategies.Single(s => s.IsUSA == isUSA);
    }
}
