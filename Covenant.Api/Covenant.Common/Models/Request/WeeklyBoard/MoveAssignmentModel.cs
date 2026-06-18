namespace Covenant.Common.Models.Request.WeeklyBoard;

public class MoveAssignmentModel
{
    public Guid RequestId { get; set; }
    public Guid FromRecruiterId { get; set; }
    public DateTime FromWorkDate { get; set; }
    public Guid ToRecruiterId { get; set; }
    public DateTime ToWorkDate { get; set; }
}
