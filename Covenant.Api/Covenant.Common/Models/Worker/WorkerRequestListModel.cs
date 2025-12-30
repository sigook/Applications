namespace Covenant.Common.Models.Worker
{
    public class WorkerRequestListModel
    {
        public Guid Id { get; set; }
        public string JobTitle { get; set; }
        public int NumberId { get; set; }
        public int WorkersQuantity { get; set; }
        public string Location { get; set; }
        public string Entrance { get; set; }
        public string AgencyFullName { get; set; }
        public string AgencyLogo { get; set; }
        public string Status { get; set; }
        public bool IsAsap { get; set; }
        public string WorkerApprovedToWork { get; set; }
        public decimal? WorkerRate { get; set; }
        public decimal? WorkerSalary { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? FinishAt { get; set; }
        public DateTime? StartAt { get; set; }
        public string DurationTerm { get; set; }
    }
}
