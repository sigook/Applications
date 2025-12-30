using Covenant.Common.Functionals;

namespace Covenant.Common.Entities.Accounting.PayStub
{
    public class PayStubPublicHoliday
    {
        private PayStubPublicHoliday() 
        { 
        }

        private PayStubPublicHoliday(DateTime holiday, decimal amount, Guid id = default)
        {
            Holiday = holiday;
            Amount = amount;
            Id = id == default ? Guid.NewGuid() : id;
        }

        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid PayStubId { get; private set; }
        public DateTime Holiday { get; private set; }
        public decimal Amount { get; private set; }
        public string Description { get; set; }
        public PayStub PayStub { get; private set; }

        public static Result<PayStubPublicHoliday> Create(DateTime holiday, decimal amount, string description = default, Guid id = default)
        {
            if (amount < decimal.Zero) return Result.Fail<PayStubPublicHoliday>("Holiday amount must be greater or equal to zero");
            return Result.Ok(new PayStubPublicHoliday(holiday, amount, id) { Description = description });
        }

        public void AssignTo(PayStub payStub)
        {
            PayStub = payStub ?? throw new ArgumentNullException();
            PayStubId = payStub.Id;
        }
    }
}