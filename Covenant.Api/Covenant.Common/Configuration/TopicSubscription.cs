namespace Covenant.Common.Configuration
{
    public class TopicSubscription
    {
        private TopicSubscription(string subscription)
        {
            Subscription = subscription;
        }

        public string Subscription { get; }

        public static implicit operator string(TopicSubscription s) => s.Subscription;

        public static TopicSubscription TeamsNotification => new(nameof(TeamsNotification));
        public static TopicSubscription EmailNotification => new(nameof(EmailNotification));
        public static TopicSubscription RequestApplicantNotification => new(nameof(RequestApplicantNotification));
    }
}
