namespace Covenant.Common.Models.Workers
{
    public interface IJobExperienceInformation<TJobExperienceModel> where TJobExperienceModel : IWorkerProfileJobExperience
    {
        IEnumerable<TJobExperienceModel> JobExperiences { get; set; }
    }
}
