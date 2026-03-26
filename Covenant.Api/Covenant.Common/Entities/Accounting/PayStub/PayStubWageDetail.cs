using Covenant.Common.Entities.Request;

namespace Covenant.Common.Entities.Accounting.PayStub;

public class PayStubWageDetail(decimal workerRate, decimal regular, decimal otherRegular, decimal missing, decimal missingOvertime, decimal nightShift, decimal holiday, decimal overtime, Guid timeSheetTotalId, Guid id = default)
{
    public Guid Id { get; set; } = id == default ? Guid.NewGuid() : id;
    public decimal WorkerRate { get; set; } = workerRate;
    public decimal Regular { get; set; } = regular;
    public decimal OtherRegular { get; set; } = otherRegular;
    public decimal Missing { get; set; } = missing;
    public decimal MissingOvertime { get; set; } = missingOvertime;
    public decimal NightShift { get; set; } = nightShift;
    public decimal Holiday { get; set; } = holiday;
    public decimal Overtime { get; set; } = overtime;
    public Guid TimeSheetTotalId { get; set; } = timeSheetTotalId;
    public TimeSheetTotalPayroll TimeSheetTotal { get; set; }
    public Guid PayStubId { get; set; }
    public PayStub PayStub { get; set; }
}