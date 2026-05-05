namespace Covenant.Common.Models.Request
{
    public class BulkRequestCancellation
    {
        public IEnumerable<Guid> Ids { get; set; }
        public Guid CancellationReasonId { get; set; }
        public string OtherCancellationReason { get; set; }
    }

    public class BulkRequestCancellationResult
    {
        public int Cancelled { get; set; }
        public int Skipped { get; set; }
    }
}
