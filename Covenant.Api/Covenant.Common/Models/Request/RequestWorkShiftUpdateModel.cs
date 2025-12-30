namespace Covenant.Common.Models.Request
{
    public class RequestWorkShiftUpdateModel
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public TimeSpan HoursDay { get; set; }
    }
}