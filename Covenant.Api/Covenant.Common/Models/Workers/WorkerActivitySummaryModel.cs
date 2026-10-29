using Covenant.Common.Models.Accounting.PayStubs;
using Covenant.Common.Models.Requests.TimeSheets;

namespace Covenant.Common.Models.Workers;

public class WorkerActivitySummaryModel
{
    public WorkerPayStubListModel LastPayStub { get; set; }
    public WorkerWeekHoursModel WeekHours { get; set; } = new();
}
