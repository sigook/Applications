namespace Covenant.Common.Entities.Worker
{
    public class WorkerProfileAvailabilityDay
    {
        public WorkerProfile WorkerProfile { get; internal set; }
        public Guid WorkerProfileId { get; private set; }
        public Day Day { get; internal set; }
        public Guid DayId { get; internal set; }
    }
}