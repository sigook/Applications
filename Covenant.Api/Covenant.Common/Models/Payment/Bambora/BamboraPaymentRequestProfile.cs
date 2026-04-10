using System.Text.Json.Serialization;

namespace Covenant.Common.Models.Payment.Bambora
{
    public class BamboraPaymentRequestProfile
    {
        [JsonPropertyName("customer_code")]
        public string CustomerCode { get; set; }

        [JsonPropertyName("card_id")]
        public int CardId { get; set; }

        [JsonPropertyName("complete")]
        public bool Complete { get; set; }

        public BamboraPaymentRequestProfile()
        {
            CardId = 1; // 1+
            Complete = true;
        }
    }
}