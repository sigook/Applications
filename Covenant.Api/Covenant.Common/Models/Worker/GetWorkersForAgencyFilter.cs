using Covenant.Common.Enums;

namespace Covenant.Common.Models.Worker
{
    public enum GetWorkersRequestForAgencySortBy
    {
        Id,
        Name,
        NumberId,
        Phone,
        RequestId,
        CreateAt
    }

    public class GetWorkersForAgencyFilter : Pagination
    {
        public string Filter { get; set; }

        public IEnumerable<WorkerStatus> Status { get; set; }

        public bool? IsWorking
        {
            get
            {
                if
                (
                    Status == null
                    || !Status.Any()
                    || (Status.Contains(WorkerStatus.Working) && Status.Contains(WorkerStatus.NotWorking))
                )
                    return null;

                return Status.Contains(WorkerStatus.Working);
            }
        }

        public GetWorkersRequestForAgencySortBy? SortBy { get; set; }
    }
}