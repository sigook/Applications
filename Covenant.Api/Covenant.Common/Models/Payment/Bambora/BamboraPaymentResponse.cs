using System.Text.Json.Serialization;

namespace Covenant.Common.Models.Payment.Bambora
{
    public class BamboraPaymentResponse
    {
        public long Code { get; set; }

        [JsonPropertyName("id")]
        public string TransactionId { get; set; }


        [JsonPropertyName("approved")]
        public string Approved { get; set; }


        [JsonPropertyName("message_id")]
        public string MessageId { get; set; }


        [JsonPropertyName("message")]
        public string Message { get; set; }


        [JsonPropertyName("auth_code")]
        public string AuthCode { get; set; }


        [JsonPropertyName("created")]
        public DateTime Created { get; set; }


        [JsonPropertyName("order_number")]
        public string OrderNumber { get; set; }


        [JsonPropertyName("type")]
        public string TransType { get; set; }


        [JsonPropertyName("payment_method")]
        public string PaymentMethod { get; set; }
    }
}