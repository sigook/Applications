using Covenant.Common.Enums;

namespace Covenant.Common.Models.Request.WeeklyBoard
{
    public class WeeklyBoardAssignmentModel
    {
        public Guid RecruiterId { get; set; }
        public string RecruiterName { get; set; }
        public Guid RequestId { get; set; }
        public int NumberId { get; set; }
        public string CompanyName { get; set; }
        public string JobTitle { get; set; }
        public DateTime WorkDate { get; set; }
        public RequestStatus Status { get; set; }
        public bool IsAsap { get; set; }
        public decimal? WorkerSalary { get; set; }
        public int WorkersSent { get; set; }
        public List<WeeklyBoardDispatchModel> Dispatches { get; set; } = [];
    }
}
