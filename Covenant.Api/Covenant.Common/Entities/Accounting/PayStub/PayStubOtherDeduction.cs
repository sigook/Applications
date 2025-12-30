namespace Covenant.Common.Entities.Accounting.PayStub
{
    public class PayStubOtherDeduction : IPayStubDeduction
    {
        private PayStubOtherDeduction()
        {
        }

        private PayStubOtherDeduction(double quantity, decimal unitPrice, string description, Guid id = default)
        {
            Id = id == default ? Guid.NewGuid() : id;
            Quantity = quantity;
            UnitPrice = unitPrice;
            Description = description;
            Total = decimal.Multiply(new decimal(quantity), unitPrice);
        }

        public Guid Id { get; private set; }
        public double Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal Total { get; private set; }
        public string Description { get; private set; }
        public Guid PayStubId { get; private set; }
        public PayStub PayStub { get; private set; }

        public void AssignTo(PayStub payStub)
        {
            PayStub = payStub ?? throw new ArgumentNullException(nameof(payStub));
            PayStubId = payStub.Id;
        }

        public static PayStubOtherDeduction CreateDefaultDeduction(decimal total, string description) =>
            new PayStubOtherDeduction(1, total, description);
    }
}