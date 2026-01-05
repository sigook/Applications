namespace Covenant.Common.Entities.Worker
{
    public class WorkerProfileAvailabilityTime
    {
        public Guid WorkerProfileId { get; set; }
        public WorkerProfile WorkerProfile { get; set; }
        public Guid AvailabilityTimeId { get; set; }
        public AvailabilityTime AvailabilityTime { get; set; }
    }
}