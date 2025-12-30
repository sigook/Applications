namespace Covenant.Common.Models.Worker
{
    public class WorkerProfileListModel
    {
        public Guid AgencyId { get; set; }
        public Guid Id { get; set; }
        public int NumberId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }
        public string ProfileImage { get; set; }
        public IEnumerable<string> Skills { get; set; } = Array.Empty<string>();
        public Guid WorkerId { get; set; }
        public bool ApprovedToWork { get; set; }
        public bool IsCurrentlyWorking { get; set; }
        public bool IsSubcontractor { get; set; }
        public bool Dnu { get; set; }
        public DateTime CreatedAt { get; set; }
        public string SinNumber { get; set; }
        public IEnumerable<BaseModel<Guid>> Requests { get; set; }
    }
}