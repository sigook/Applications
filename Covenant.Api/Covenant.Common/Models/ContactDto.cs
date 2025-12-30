using Covenant.Common.Enums;

namespace Covenant.Common.Models
{
    public class ContactDto
    {
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string Industry { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }
        public string Subject { get; set; }
        public string CaptchaResponse { get; set; }
        public EmailSettingName EmailSetting { get; set; }
    }
}
