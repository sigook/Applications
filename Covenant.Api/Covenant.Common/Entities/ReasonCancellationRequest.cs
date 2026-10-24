namespace Covenant.Common.Entities
{
    public class ReasonCancellationRequest
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Value { get; private set; }
        public string Code { get; private set; }
    }
}