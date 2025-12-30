namespace Covenant.Common.Entities.Worker
{
    public class WorkerProfileAvailability : IEquatable<WorkerProfileAvailability>
    {
        internal WorkerProfileAvailability()
        {
        }

        public WorkerProfileAvailability(Guid workerProfileId, Guid availabilityId)
        {
            WorkerProfileId = workerProfileId;
            AvailabilityId = availabilityId;
        }

        public Guid WorkerProfileId { get; internal set; }
        internal WorkerProfile WorkerProfile { get; set; }
        public Guid AvailabilityId { get; internal set; }
        public Availability Availability { get; set; }

        public bool Equals(WorkerProfileAvailability other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return WorkerProfileId.Equals(other.WorkerProfileId) && AvailabilityId.Equals(other.AvailabilityId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((WorkerProfileAvailability)obj);
        }

        public override int GetHashCode() => HashCode.Combine(WorkerProfileId, AvailabilityId);

        public static bool operator ==(WorkerProfileAvailability left, WorkerProfileAvailability right) => Equals(left, right);

        public static bool operator !=(WorkerProfileAvailability left, WorkerProfileAvailability right) => !Equals(left, right);
    }
}