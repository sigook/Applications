using Covenant.Common.Enums;

namespace Covenant.Common.Entities.Workers;

public class WorkerProfileTaxCategory
{
    private WorkerProfileTaxCategory()
    {
    }

    public WorkerProfileTaxCategory(Guid workerProfileId) => WorkerProfileId = workerProfileId;

    public Guid WorkerProfileId { get; private set; }
    public TaxCategory? FederalCategory { get; internal set; }
    public TaxCategory? ProvincialCategory { get; private set; }
    public decimal? Cpp { get; private set; }
    public decimal? Ei { get; private set; }
    public WorkerProfile WorkerProfile { get; private set; }

    public void UpdateCategories(TaxCategory? federalCategory, TaxCategory? provincialCategory)
    {
        FederalCategory = federalCategory;
        ProvincialCategory = provincialCategory;
    }

    public void UpdateRates(decimal? cpp, decimal? ei)
    {
        Cpp = cpp;
        Ei = ei;
    }
}
