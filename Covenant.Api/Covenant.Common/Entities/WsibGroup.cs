namespace Covenant.Common.Entities
{
    public class WsibGroup
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Value { get; private set; }
        public int RateGroup { get; private set; }
        public double Rate { get; private set; }
    }
}