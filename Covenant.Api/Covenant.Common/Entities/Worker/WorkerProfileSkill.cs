namespace Covenant.Common.Entities.Worker
{
    public class WorkerProfileSkill
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public WorkerProfile WorkerProfile { get; internal set; }
        public Guid WorkerProfileId { get; private set; }
        public string Skill { get; internal set; }
    }
}