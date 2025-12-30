namespace Covenant.Common.Models.Accounting;

public class HoursWorkedFilter : Pagination
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? CompanyId { get; set; }
    public Guid? JobPositionRateId { get; set; }
}
