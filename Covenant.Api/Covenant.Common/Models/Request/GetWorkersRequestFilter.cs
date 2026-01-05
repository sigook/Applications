using Covenant.Common.Enums;
using Covenant.Common.Models;

namespace Covenant.Common.Models.Request
{
    public enum GetWorkersRequestSortBy
    {
        NumberId,
        Name,
        Status,
        StartWorking,
        CreatedBy,
        RejectedBy
    }

    public class GetWorkersRequestFilter : Pagination
    {
        public GetWorkersRequestSortBy SortBy { get; set; }
        public int? NumberId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string SocialInsurance { get; set; }
        public DateTime? StartWorkingFrom { get; set; }
        public DateTime? StartWorkingTo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAtFrom { get; set; }
        public DateTime? CreatedAtTo { get; set; }
        public string RejectedBy { get; set; }
        public DateTime? RejectedAtFrom { get; set; }
        public DateTime? RejectedAtTo { get; set; }
        public IEnumerable<WorkerRequestStatus> Statuses { get; set; }
    }
}