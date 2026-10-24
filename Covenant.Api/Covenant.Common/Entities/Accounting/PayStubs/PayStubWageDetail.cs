using Covenant.Common.Entities.Requests;

namespace Covenant.Common.Entities.Accounting.PayStubs;

public class PayStubWageDetail(decimal workerRate, decimal regular, decimal otherRegular, decimal missing, decimal missingOvertime, decimal nightShift, decimal holiday, decimal overtime, Guid timeSheetTotalId, Guid id = default)
{
    public Guid Id { get; private set; } = id == default ? Guid.NewGuid() : id;
    public decimal WorkerRate { get; private set; } = workerRate;
    public decimal Regular { get; private set; } = regular;
    public decimal OtherRegular { get; private set; } = otherRegular;
    public decimal Missing { get; private set; } = missing;
    public decimal MissingOvertime { get; private set; } = missingOvertime;
    public decimal NightShift { get; private set; } = nightShift;
    public decimal Holiday { get; private set; } = holiday;
    public decimal Overtime { get; private set; } = overtime;
    public Guid TimeSheetTotalId { get; private set; } = timeSheetTotalId;
    public TimeSheetTotalPayroll TimeSheetTotal { get; private set; }
    public Guid PayStubId { get; private set; }
    public PayStub PayStub { get; private set; }

    public PayStubWageDetail AttachTimeSheetTotal(TimeSheetTotalPayroll timeSheetTotal)
    {
        TimeSheetTotal = timeSheetTotal;
        return this;
    }
}
