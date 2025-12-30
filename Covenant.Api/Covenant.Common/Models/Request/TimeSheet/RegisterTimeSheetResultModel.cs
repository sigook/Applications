namespace Covenant.Common.Models.Request.TimeSheet;

public class RegisterTimeSheetResultModel
{
    public RegisterTimeSheetResultModel()
    {
    }

    public RegisterTimeSheetResultModel(Guid timeSheetId, string workerFullName, bool finish)
    {
        TimeSheetId = timeSheetId;
        WorkerFullName = workerFullName;
        Finish = finish;
    }

    public Guid TimeSheetId { get; set; }
    public string WorkerFullName { get; set; }
    public bool Finish { get; set; }

    public override string ToString() => $"{WorkerFullName}|{Finish}";
}