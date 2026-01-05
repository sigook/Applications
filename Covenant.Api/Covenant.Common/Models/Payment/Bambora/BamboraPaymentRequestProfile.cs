using Newtonsoft.Json;

namespace Covenant.Common.Models.Payment.Bambora
{
    public class BamboraPaymentRequestProfile
    {
        [JsonProperty(PropertyName = "customer_code")]
        public string CustomerCode { get; set; }

        [JsonProperty(PropertyName = "card_id")]
        public int CardId { get; set; }

        [JsonProperty(PropertyName = "complete")]
        public bool Complete { get; set; }

        public BamboraPaymentRequestProfile()
        {
            CardId = 1; // 1+
            Complete = true;
        }
    }
}