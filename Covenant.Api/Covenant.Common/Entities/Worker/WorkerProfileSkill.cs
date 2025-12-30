namespace Covenant.Common.Entities.Worker
{
    public class WorkerProfileSkill
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public WorkerProfile WorkerProfile { get; set; }
        public Guid WorkerProfileId { get; set; }
        public string Skill { get; set; }
    }
}