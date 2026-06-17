using Covenant.Common.Interfaces;
using Covenant.Common.Repositories.Agency;
using Covenant.Core.BL.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Covenant.Core.BL.Services.Invoices;

public class InvoiceServiceFactory(
    IServiceProvider serviceProvider,
    IAgencyRepository agencyRepository,
    IIdentityServerService identityServerService)
{
    public async Task<IInvoiceService> Resolve()
    {
        var agencyId = identityServerService.GetAgencyId();
        var billingLocation = await agencyRepository.GetBillingLocation(agencyId);

        return billingLocation?.IsUSA == true
            ? serviceProvider.GetRequiredService<UsaInvoiceService>()
            : serviceProvider.GetRequiredService<CanadaInvoiceService>();
    }
}
