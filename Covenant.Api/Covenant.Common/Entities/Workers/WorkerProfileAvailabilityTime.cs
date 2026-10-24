namespace Covenant.Common.Entities.Workers
{
    public class WorkerProfileAvailabilityTime
    {
        public Guid WorkerProfileId { get; private set; }
        public WorkerProfile WorkerProfile { get; internal set; }
        public Guid AvailabilityTimeId { get; internal set; }
        public AvailabilityTime AvailabilityTime { get; internal set; }
    }
}