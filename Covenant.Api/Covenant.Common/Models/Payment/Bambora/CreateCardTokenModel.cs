using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Covenant.Common.Models.Payment.Bambora
{
    public class CreateCardTokenModel
    {
        [Required]
        public string Number { get; set; }
        [Required]
        [JsonPropertyName("expiry_month")]
        public int ExpiryMonth { get; set; }
        [Required]
        [JsonPropertyName("expiry_year")]
        public int ExpiryYear { get; set; }
        [Required]
        public int Cvd { get; set; }
    }
}