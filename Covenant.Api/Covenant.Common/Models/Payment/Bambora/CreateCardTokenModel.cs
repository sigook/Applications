using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Covenant.Common.Models.Payment.Bambora
{
    public class CreateCardTokenModel
    {
        [Required]
        public string Number { get; set; }
        [Required]
        [JsonProperty("expiry_month")]
        public int ExpiryMonth { get; set; }
        [Required]
        [JsonProperty("expiry_year")]
        public int ExpiryYear { get; set; }
        [Required]
        public int Cvd { get; set; }
    }
}