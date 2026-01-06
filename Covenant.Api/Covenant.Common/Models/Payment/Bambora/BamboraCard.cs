using Newtonsoft.Json;

namespace Covenant.Common.Models.Payment.Bambora
{
    public class BamboraCard
    {
        [JsonProperty("card_id")]
        public int CardId { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        [JsonProperty("expiry_month")]
        public string ExpiryMonth { get; set; }
        [JsonProperty("expiry_year")]
        public string ExpiryYear { get; set; }
        [JsonProperty("card_type")]
        public string CardType { get; set; }
    }
}