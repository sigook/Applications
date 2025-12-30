using Covenant.Common.Enums;

namespace Covenant.Common.Configuration
{
    public class EmailSettings
    {
        public EmailSettingName EmailSettingName { get; set; }
        public string DisplayName { get; set; }
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