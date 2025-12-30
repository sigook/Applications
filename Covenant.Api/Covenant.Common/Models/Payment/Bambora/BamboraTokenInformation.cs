namespace Covenant.Common.Models.Payment.Bambora
{
	public class BamboraTokenInformation
	{
		public string Name { get; set; }
		public string Code { get; set; }
		/// <summary>
		/// For payment profiles, identified if this card is DEF (default) or SEC (secondary).
		/// This value is optional.
		/// </summary>
		public string Function { get; set; }
		public override string ToString() => $"Name={Name},Code={Code},Function={Function}";
	}
}