using Covenant.Common.Entities;

namespace Covenant.Common.Models.Request
{
    public class WorkerRequestInfoModel
    {
        public string WorkerFullName { get; set; }
        public Guid WorkerRequestId { get; set; }
        public Shift Shift { get; set; }
        public DateTime? StartWorking { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string CountryCode { get; set; }
    }
}