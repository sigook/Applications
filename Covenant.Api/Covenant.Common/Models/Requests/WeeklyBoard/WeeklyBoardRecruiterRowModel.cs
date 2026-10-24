namespace Covenant.Common.Models.Requests.WeeklyBoard
{
    public class WeeklyBoardRecruiterRowModel
    {
        public Guid RecruiterId { get; set; }
        public string RecruiterName { get; set; }
        public int OrdersCount { get; set; }
        public int WorkersSent { get; set; }
        public IEnumerable<WeeklyBoardAssignmentModel> Assignments { get; set; } = new List<WeeklyBoardAssignmentModel>();
    }
}
