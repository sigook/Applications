using Covenant.Common.Entities.Request;

namespace Covenant.Common.Entities.Accounting.PayStub
{
    public class PayStubWageDetail
    {
        private PayStubWageDetail()
        {
        }

        public PayStubWageDetail(decimal workerRate, decimal regular, decimal otherRegular, decimal missing, decimal missingOvertime, decimal nightShift, decimal holiday, decimal overtime, Guid timeSheetTotalId, Guid id = default)
        {
            WorkerRate = workerRate;
            Regular = regular;
            OtherRegular = otherRegular;
            Missing = missing;
            MissingOvertime = missingOvertime;
            NightShift = nightShift;
            Holiday = holiday;
            Overtime = overtime;
            TimeSheetTotalId = timeSheetTotalId;
            Id = id == default ? Guid.NewGuid() : id;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public decimal WorkerRate { get; set; }
        public decimal Regular { get; set; }
        public decimal OtherRegular { get; set; }
        public decimal Missing { get; set; }
        public decimal MissingOvertime { get; set; }
        public decimal NightShift { get; set; }
        public decimal Holiday { get; set; }
        public decimal Overtime { get; set; }
        public Guid TimeSheetTotalId { get; set; }
        public TimeSheetTotalPayroll TimeSheetTotal { get; set; }
        public Guid PayStubId { get; set; }
        public PayStub PayStub { get; set; }

        public void AssignTo(PayStub payStub)
        {
            PayStub = payStub ?? throw new ArgumentNullException(nameof(payStub));
            PayStubId = payStub.Id;
        }
    }
}