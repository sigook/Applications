namespace Covenant.Common.Models.Notification
{
    public class UnsubscribeModel
    {
        public Guid UserId { get; set; }
        public string TypeId { get; set; }
    }
}