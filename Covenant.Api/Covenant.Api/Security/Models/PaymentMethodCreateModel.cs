namespace Covenant.Api.Security.Models
{
    public class PaymentMethodCreateModel
    {
        public string StripeToken { get; set; }
        public string CardToken { get; set; }
        public string Name { get; set; }
    }
}