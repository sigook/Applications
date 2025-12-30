using Covenant.Common.Enums;

namespace Covenant.Common.Models
{
    public class EmailParams
    {
        public EmailParams()
        {
        }

        public EmailParams(string email, string subject, string message)
        {
            Email = email;
            Subject = subject;
            Message = message;
        }

        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public List<string> Cc { get; set; } = new List<string>();
        public List<string> Bcc { get; set; } = new List<string>();
        public IEnumerable<EmailAttachment> Attachments { get; set; } = new List<EmailAttachment>();
        public EmailSettingName EmailSettingName { get; set; } = EmailSettingName.Default;
    }
}
