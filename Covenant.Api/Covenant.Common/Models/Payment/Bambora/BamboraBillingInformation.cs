using Newtonsoft.Json;

namespace Covenant.Common.Models.Payment.Bambora
{
    public class BamboraBillingInformation
    {
        public string Name { get; set; }
        [JsonProperty("address_line1")]
        public string AddressLine1 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        [JsonProperty("postal_code")]
        public string PostalCode { get; set; }
        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }
        [JsonProperty("email_address")]
        public string EmailAddress { get; set; }

        public override string ToString() =>
            $"Name={Name},AddressLine1={AddressLine1},City={City},Province={Province},Country={Country},PostalCode={PostalCode},PhoneNumber={PhoneNumber},EmailAddress={EmailAddress}";
    }
}