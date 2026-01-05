using Newtonsoft.Json;

namespace Covenant.Common.Models.Payment.Bambora
{
    public class BamboraPaymentResponse
    {
        public long Code { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string TransactionId { get; set; }


        [JsonProperty(PropertyName = "approved")]
        public string Approved { get; set; }


        [JsonProperty(PropertyName = "message_id")]
        public string MessageId { get; set; }


        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }


        [JsonProperty(PropertyName = "auth_code")]
        public string AuthCode { get; set; }


        [JsonProperty(PropertyName = "created")]
        public DateTime Created { get; set; }


        [JsonProperty(PropertyName = "order_number")]
        public string OrderNumber { get; set; }


        [JsonProperty(PropertyName = "type")]
        public string TransType { get; set; }


        [JsonProperty(PropertyName = "payment_method")]
        public string PaymentMethod { get; set; }
    }
}