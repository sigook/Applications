namespace Covenant.Common.Entities.Request
{
    public class RequestSource
    {
        public Guid RequestId { get; set; }
        public Request Request { get; set; }
        public Guid SourceId { get; set; }
        public Source Source { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string ExternalUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
