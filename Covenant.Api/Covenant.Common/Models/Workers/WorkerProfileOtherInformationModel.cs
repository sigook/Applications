namespace Covenant.Common.Models.Workers;

public class WorkerProfileOtherInformationModel : IWorkerProfileOtherInformation<BaseModel<Guid>>
{
    public BaseModel<Guid> Lift { get; set; }
}
