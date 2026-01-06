using Newtonsoft.Json;

namespace Covenant.Common.Models.Payment.Bambora
{
	public class BamboraProfileResponse
	{
		public long Code { get; set; }
		public string Message { get; set; }
		[JsonProperty("customer_code")] 
		public string CustomerCode { get; set; }
		public override string ToString() => $"Code={Code},Message={Message}";
	}
}