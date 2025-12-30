namespace Covenant.Common.Entities.Worker
{
    public class WorkerProfileLocationPreference
    {
        public WorkerProfile WorkerProfile { get; set; }
        public Guid WorkerProfileId { get; set; }
        public City City { get; set; }
        public Guid CityId { get; set; }
    }
}