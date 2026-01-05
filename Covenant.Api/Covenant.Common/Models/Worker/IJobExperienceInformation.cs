namespace Covenant.Common.Models.Worker
{
    public interface IJobExperienceInformation<TJobExperienceModel> where TJobExperienceModel : IWorkerProfileJobExperience
    {
        IEnumerable<TJobExperienceModel> JobExperiences { get; set; }
    }
}
