using Newtonsoft.Json;

namespace Covenant.Common.Models.Payment.Bambora
{
    public class BamboraCardResponse
    {
        public long Code { get; set; }
        public string Message { get; set; }
        [JsonProperty("customer_code")]
        public string CustomerCode { get; set; }
        public List<BamboraCard> Card { get; set; } = new List<BamboraCard>();
    }
}