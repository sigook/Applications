namespace Covenant.Common.Entities
{
    public class ReasonCancellationRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public StringResource Value { get; set; }
        public string Code { get; set; }
    }
}