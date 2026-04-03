namespace Sigook.Functions.Models;

public class SendGridSettings
{
    public string FromEmailAddress { get; set; }
    public string TestingEmailAddress { get; set; }
    public string SendGridApiKey { get; set; }
    public string NewJobTemplateId { get; set; }
    public string NewApplicantTemplateId { get; set; }
    public string UnsubscribeUrl { get; set; }
    public string ApplyOnlineUrl { get; set; }
}
