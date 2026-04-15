using System.Text.Json.Serialization;

namespace Covenant.Common.Models.Payment.Bambora
{
    public class BamboraBillingInformation
    {
        public string Name { get; set; }
        [JsonPropertyName("address_line1")]
        public string AddressLine1 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        [JsonPropertyName("postal_code")]
        public string PostalCode { get; set; }
        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; }
        [JsonPropertyName("email_address")]
        public string EmailAddress { get; set; }

        public override string ToString() =>
            $"Name={Name},AddressLine1={AddressLine1},City={City},Province={Province},Country={Country},PostalCode={PostalCode},PhoneNumber={PhoneNumber},EmailAddress={EmailAddress}";
    }
}