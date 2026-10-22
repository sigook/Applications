namespace Covenant.Common.Entities.Request
{
    public class RequestFinalizationDetail
    {
        private RequestFinalizationDetail() 
        { 
        }

        public RequestFinalizationDetail(Guid requestId, string finalizedBy, string detail, DateTime finalizedAt)
        {
            RequestId = requestId;
            FinalizedBy = finalizedBy;
            Detail = detail;
            FinalizedAt = finalizedAt;
        }

        public Guid Id { get; private set; } = Guid.NewGuid();
        public Request Request { get; private set; }
        public Guid RequestId { get; private set; }
        public string Detail { get; private set; }
        public string FinalizedBy { get; private set; }
        public DateTime FinalizedAt { get; private set; }
    }
}