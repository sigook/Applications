namespace Covenant.Common.Models.Worker
{
    public interface IWorkerProfileJobExperience
    {
        string Company { get; }
        string Supervisor { get; }
        string Duties { get; }
        DateTime StartDate { get; }
        DateTime? EndDate { get; }
        bool IsCurrentJobPosition { get; }
    }
}
