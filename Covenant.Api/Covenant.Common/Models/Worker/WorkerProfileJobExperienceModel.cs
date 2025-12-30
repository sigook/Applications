namespace Covenant.Common.Models.Worker
{
    public class WorkerProfileJobExperienceModel : IWorkerProfileJobExperience
    {
        public string Company { get; set; }
        public string Supervisor { get; set; }
        public string Duties { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrentJobPosition { get; set; }
    }
}
