using Covenant.Common.Enums;

namespace Covenant.Common.Entities.Worker;

public class WorkerProfileTaxCategory
{
    public Guid WorkerProfileId { get; set; }
    public TaxCategory? FederalCategory { get; set; }
    public TaxCategory? ProvincialCategory { get; set; }
    public decimal? Cpp { get; set; }
    public decimal? Ei { get; set; }
    public WorkerProfile WorkerProfile { get; set; }
}
