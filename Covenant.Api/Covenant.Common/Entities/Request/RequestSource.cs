namespace Covenant.Common.Entities.Request
{
    public class RequestSource
    {
        internal RequestSource()
        {
        }

        public RequestSource(Guid requestId, Guid sourceId, DateTime? publishedAt, string externalUrl)
        {
            RequestId = requestId;
            SourceId = sourceId;
            PublishedAt = publishedAt;
            ExternalUrl = externalUrl;
        }

        public Guid RequestId { get; private set; }
        public Request Request { get; private set; }
        public Guid SourceId { get; internal set; }
        public Source Source { get; private set; }
        public DateTime? PublishedAt { get; internal set; }
        public string ExternalUrl { get; internal set; }
        public DateTime CreatedAt { get; private set; } = DateTime.Now;
    }
}
