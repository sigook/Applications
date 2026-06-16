namespace Covenant.Common.Models.Request.WeeklyBoard
{
    public class AssignRecruitersModel
    {
        public Guid RequestId { get; set; }
        public IEnumerable<DateTime> WorkDates { get; set; }
        public IEnumerable<Guid> RecruiterIds { get; set; }
    }
}
