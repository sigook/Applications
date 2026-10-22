namespace Covenant.Common.Models.Requests.WeeklyBoard
{
    public class AssignRecruitersModel
    {
        public Guid RequestId { get; set; }
        public IEnumerable<DateTime> WorkDates { get; set; }
        public IEnumerable<Guid> RecruiterIds { get; set; }
    }
}
