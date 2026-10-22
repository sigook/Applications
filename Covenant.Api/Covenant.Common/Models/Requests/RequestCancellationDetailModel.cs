namespace Covenant.Common.Models.Request
{
    public class RequestCancellationDetailModel
    {
        public Guid CancellationReasonId { get; set; }
        public string OtherCancellationReason { get; set; }
    }
}