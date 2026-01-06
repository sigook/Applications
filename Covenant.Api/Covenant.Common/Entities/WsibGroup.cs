namespace Covenant.Common.Entities
{
    public class WsibGroup
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Value { get; set; }
        public int RateGroup { get; set; }
        public double Rate { get; set; }
    }
}