namespace Covenant.Common.Configuration;

public class SendGridConfiguration
{
    public string ApiKey { get; set; }
    public string TemplatesUrl { get; set; }
    public string TestingEmailAddress { get; set; }
    public string NewJobTemplateId { get; set; }
    public string NewApplicantTemplateId { get; set; }
    public string UnsubscribeUrl { get; set; }
    public string ApplyOnlineUrl { get; set; }
    public string CandidateApplyOnlineUrl { get; set; }
    public bool RedirectInvitationsToTestingEmail { get; set; }
}
