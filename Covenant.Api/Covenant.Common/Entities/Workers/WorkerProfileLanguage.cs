namespace Covenant.Common.Entities.Workers
{
    public class WorkerProfileLanguage
    {
        public WorkerProfile WorkerProfile { get; internal set; }
        public Guid WorkerProfileId { get; private set; }
        public Language Language { get; internal set; }
        public Guid LanguageId { get; internal set; }
    }
}