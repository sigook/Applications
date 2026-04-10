using System.Text.Json.Serialization;

namespace Covenant.Common.Models.Payment.Bambora
{
    public class BamboraCardResponse
    {
        public long Code { get; set; }
        public string Message { get; set; }
        [JsonPropertyName("customer_code")]
        public string CustomerCode { get; set; }
        public List<BamboraCard> Card { get; set; } = new List<BamboraCard>();
    }
}