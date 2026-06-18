namespace Covenant.Common.Models.Request.WeeklyBoard
{
    public class WeeklyBoardModel
    {
        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }
        public int TotalAssignments { get; set; }
        public int TotalWorkersSent { get; set; }
        public IEnumerable<WeeklyBoardRecruiterRowModel> Recruiters { get; set; } = new List<WeeklyBoardRecruiterRowModel>();
    }
}
