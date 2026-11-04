using Covenant.Common.Enums;

namespace Covenant.Common.Models.Notifications;

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
    public List<string> Cc { get; set; } = [];
    public List<string> Bcc { get; set; } = [];
    public IEnumerable<EmailAttachment> Attachments { get; set; } = [];
    public EmailSettingName EmailSettingName { get; set; } = EmailSettingName.Default;
}
