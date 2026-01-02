namespace Covenant.IdentityServer.Services.Models
{
	public class EmailSettings
	{
		public string PrimaryDomain { get; set; }
		public int PrimaryPort { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string FromEmail { get; set; }
		public string ToEmail { get; set; }
		public string CcEmail { get; set; }
		public bool Test { get; set; }
		public string TestEmails { get; set; }
	}
}