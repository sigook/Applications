namespace Covenant.Common.Models.Payment.Bambora
{
	public class BamboraPaymentProfileInformation
	{
		public BamboraTokenInformation Token { get; set; }
		public BamboraBillingInformation Billing { get; set; }
		public BamboraCustomField Custom { get; set; }

		public override string ToString() => $"Token={Token},Billing={Billing},Custom={Custom}";
	}
}