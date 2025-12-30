namespace Covenant.Common.Entities.Request
{
    public class RequestCancellationDetail
    {
        public Guid RequestId { get; set; }
        public ReasonCancellationRequest ReasonCancellationRequest { get; set; }
        public Guid? ReasonCancellationRequestId { get; set; }
        public string OtherReasonCancellationRequest { get; set; }
        public string CancelBy { get; set; }
        public DateTime? CancelAt { get; set; }
    }
}