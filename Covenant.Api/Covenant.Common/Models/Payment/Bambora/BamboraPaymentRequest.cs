using System.Text.Json.Serialization;

namespace Covenant.Common.Models.Payment.Bambora
{
    public class BamboraPaymentRequest
    {
        [JsonPropertyName("payment_profile")]
        public BamboraPaymentRequestProfile PaymentProfile { get; set; }

        [JsonPropertyName("payment_method")]
        public string PaymentMethod { get; set; }

        [JsonPropertyName("order_number")]
        public string OrderNumber { get; set; }

        /// <summary>
        /// In the format 0.00. Max 2 decimal places. Max 9 digits total.
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// 3 characters, either ENG or FRE.
        /// Optional
        /// </summary>
        [JsonPropertyName("language")]
        public string Language { get; set; }


        /// <summary>
        /// IP address of the customer.
        /// Optional.
        /// </summary>
        /// <value>The customer ip.</value>
        [JsonPropertyName("customer_ip")]
        public string CustomerIp { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

        [JsonPropertyName("custom")]
        public BamboraCustomField CustomFields { get; set; }
    }
}