namespace Covenant.Common.Entities.Worker
{
    public class WorkerProfileLanguage
    {
        public WorkerProfile WorkerProfile { get; set; }
        public Guid WorkerProfileId { get; set; }
        public Language Language { get; set; }
        public Guid LanguageId { get; set; }
    }
}