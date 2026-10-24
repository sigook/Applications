namespace Covenant.Common.Models.Requests
{
    public class RequestCancellationDetailModel
    {
        public Guid CancellationReasonId { get; set; }
        public string OtherCancellationReason { get; set; }
    }
}