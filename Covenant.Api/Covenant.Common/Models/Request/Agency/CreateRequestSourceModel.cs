namespace Covenant.Common.Models.Request.Agency;

public class CreateRequestSourceModel
{
    public Guid SourceId { get; set; }
    public DateTime? PublishedAt { get; set; }
    public string ExternalUrl { get; set; }
}
