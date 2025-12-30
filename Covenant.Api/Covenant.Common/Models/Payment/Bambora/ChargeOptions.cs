namespace Covenant.Common.Models.Payment.Bambora
{
    public class ChargeOptions
    {
        public ChargeOptions(decimal amount, string description)
        {
            Amount = amount;
            Description = description;
        }
        public decimal Amount { get; private set; }
        public string Description { get; private set; }
        public Guid InternalTransactionId { get; set; } = Guid.NewGuid();
    }
}