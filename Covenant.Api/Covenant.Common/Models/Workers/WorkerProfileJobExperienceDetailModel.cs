namespace Covenant.Common.Models.Worker
{
    public class WorkerProfileJobExperienceDetailModel : IWorkerProfileJobExperience
    {
        public Guid Id { get; set; }
        public string Company { get; set; }
        public string Supervisor { get; set; }
        public string Duties { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrentJobPosition { get; set; }
    }
}
