namespace Covenant.Common.Models.Request
{
    public class CreateRequestSourceModel
    {
        public Guid SourceId { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string ExternalUrl { get; set; }
    }
}
