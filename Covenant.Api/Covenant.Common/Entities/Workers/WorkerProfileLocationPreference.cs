namespace Covenant.Common.Entities.Workers
{
    public class WorkerProfileLocationPreference
    {
        public WorkerProfile WorkerProfile { get; internal set; }
        public Guid WorkerProfileId { get; private set; }
        public City City { get; internal set; }
        public Guid CityId { get; internal set; }
    }
}