namespace Covenant.Common.Entities.Worker
{
    public class WorkerProfileAvailabilityDay
    {
        public WorkerProfile WorkerProfile { get; set; }
        public Guid WorkerProfileId { get; set; }
        public Day Day { get; set; }
        public Guid DayId { get; set; }
    }
}