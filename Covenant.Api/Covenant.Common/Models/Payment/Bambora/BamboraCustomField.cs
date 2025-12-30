namespace Covenant.Common.Models.Payment.Bambora
{
	public class BamboraCustomField
	{
		public string Ref1 { get; set; }
		public string Ref2 { get; set; }
		public override string ToString() => $"Ref1={Ref1},Ref2={Ref2}";
	}
}