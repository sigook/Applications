using Newtonsoft.Json;

namespace Covenant.Common.Models.Payment.Bambora
{
    public class BamboraPaymentRequest
    {
        [JsonProperty(PropertyName = "payment_profile")]
        public BamboraPaymentRequestProfile PaymentProfile { get; set; }

        [JsonProperty(PropertyName = "payment_method")]
        public string PaymentMethod { get; set; }

        [JsonProperty(PropertyName = "order_number")]
        public string OrderNumber { get; set; }

        /// <summary>
        /// In the format 0.00. Max 2 decimal places. Max 9 digits total.
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// 3 characters, either ENG or FRE.
        /// Optional
        /// </summary>
        [JsonProperty(PropertyName = "language")]
        public string Language { get; set; }


        /// <summary>
        /// IP address of the customer.
        /// Optional.
        /// </summary>
        /// <value>The customer ip.</value>
        [JsonProperty(PropertyName = "customer_ip")]
        public string CustomerIp { get; set; }

        [JsonProperty(PropertyName = "comments")]
        public string Comments { get; set; }

        [JsonProperty(PropertyName = "custom")]
        public BamboraCustomField CustomFields { get; set; }
    }
}