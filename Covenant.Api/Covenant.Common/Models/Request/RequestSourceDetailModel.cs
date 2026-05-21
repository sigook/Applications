namespace Covenant.Common.Models.Request
{
    public class RequestSourceDetailModel
    {
        public Guid SourceId { get; set; }
        public string Value { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string ExternalUrl { get; set; }
    }
}
