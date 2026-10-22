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
        public string City { get; set; }
        public string ProvinceCode { get; set; }
        public DateTime WorkDate { get; set; }
        public RequestStatus Status { get; set; }
        public bool IsAsap { get; set; }
        public decimal? WorkerSalary { get; set; }
        public bool UsesRunners { get; set; }
        public int RunnersSent { get; set; }
        public List<WeeklyBoardRunnerModel> Runners { get; set; } = [];
    }
}
