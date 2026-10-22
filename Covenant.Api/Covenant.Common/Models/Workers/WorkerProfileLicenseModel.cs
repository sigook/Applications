namespace Covenant.Common.Models.Worker;

public class WorkerProfileLicenseModel : IWorkerProfileLicense<CovenantFileModel>
{
    public CovenantFileModel License { get; set; }
    public string Number { get; set; }
    public DateTime? Issued { get; set; }
    public DateTime? Expires { get; set; }
}
