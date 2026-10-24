namespace Covenant.Common.Configuration
{
    public class ServiceBusConfiguration
    {
        public string ValidateCandidateQueue { get; set; }
        public string CreateApplicantTopic { get; set; }
        public string BulkPayStubEmailQueue { get; set; }
        public string InvitationQueue { get; set; }
    }
}
