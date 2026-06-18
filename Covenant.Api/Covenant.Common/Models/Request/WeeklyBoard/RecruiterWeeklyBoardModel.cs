namespace Covenant.Common.Models.Request.WeeklyBoard;

public class RecruiterWeeklyBoardModel
{
    public string RecruiterName { get; set; }
    public DateTime WeekStart { get; set; }
    public DateTime WeekEnd { get; set; }
    public int OrdersCount { get; set; }
    public int WorkersSent { get; set; }
    public List<WeeklyBoardAssignmentModel> Assignments { get; set; } = [];
}
