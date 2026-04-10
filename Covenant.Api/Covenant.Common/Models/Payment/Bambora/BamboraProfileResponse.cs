using System.Text.Json.Serialization;

namespace Covenant.Common.Models.Payment.Bambora
{
	public class BamboraProfileResponse
	{
		public long Code { get; set; }
		public string Message { get; set; }
		[JsonPropertyName("customer_code")]
		public string CustomerCode { get; set; }
		public override string ToString() => $"Code={Code},Message={Message}";
	}
}