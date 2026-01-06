namespace Covenant.Common.Models.Payment.Bambora
{
    public class ChargeResult
    {
        public ChargeResult(string transactionId) => TransactionId = transactionId;
        public ChargeResult(string transactionId, string rawResponse)
        {
            TransactionId = transactionId;
            RawResponse = rawResponse;
        }
        public string TransactionId { get; private set; }
        public string RawResponse { get; set; }
    }
}