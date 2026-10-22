namespace Covenant.Common.Entities.Request
{
    public class RequestCancellationDetail
    {
        internal RequestCancellationDetail()
        {
        }

        public RequestCancellationDetail(Guid requestId, Guid? reasonCancellationRequestId, string otherReasonCancellationRequest, string cancelBy, DateTime? cancelAt)
        {
            RequestId = requestId;
            ReasonCancellationRequestId = reasonCancellationRequestId;
            OtherReasonCancellationRequest = otherReasonCancellationRequest;
            CancelBy = cancelBy;
            CancelAt = cancelAt;
        }

        public Guid RequestId { get; private set; }
        public ReasonCancellationRequest ReasonCancellationRequest { get; private set; }
        public Guid? ReasonCancellationRequestId { get; private set; }
        public string OtherReasonCancellationRequest { get; private set; }
        public string CancelBy { get; private set; }
        public DateTime? CancelAt { get; private set; }
    }
}