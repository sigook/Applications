namespace Covenant.Core.BL.Interfaces.Accounting.Invoices;

public interface IInvoiceCountryStrategyResolver
{
    Task<IInvoiceCountryStrategy> ResolveForAgency(Guid agencyId);
}
