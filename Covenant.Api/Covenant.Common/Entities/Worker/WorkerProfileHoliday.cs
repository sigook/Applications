namespace Covenant.Common.Entities.Worker
{
    public class WorkerProfileHoliday
    {
        private decimal _statPaidWorker;

        private WorkerProfileHoliday() 
        { 
        }

        public WorkerProfileHoliday(Guid workerProfileId, Guid holidayId, decimal statPaidWorker, Guid id = default)
        {
            WorkerProfileId = workerProfileId;
            HolidayId = holidayId;
            StatPaidWorker = statPaidWorker;
            Id = id == default ? Guid.NewGuid() : id;
        }

        public Guid Id { get; private set; }
        public WorkerProfile WorkerProfile { get; set; }
        public Guid WorkerProfileId { get; private set; }
        public Holiday Holiday { get; set; }
        public Guid HolidayId { get; private set; }
        public decimal StatPaidWorker
        {
            get => _statPaidWorker;
            private set => _statPaidWorker = value < 0 ? 0 : value;
        }

        public void UpdateStats(decimal statPaidWorker)
        {
            StatPaidWorker = statPaidWorker;
        }
    }
}