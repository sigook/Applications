namespace Covenant.Common.Models.Worker
{
    public class WorkerProfileOtherInformationModel : IWorkerProfileOtherInformation<BaseModel<Guid>>
    {
        public BaseModel<Guid> Lift { get; set; }
    }
}