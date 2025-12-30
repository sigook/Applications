using Covenant.Common.Entities;

namespace Covenant.Common.Models.Worker
{
    public interface IWorkerProfileOtherInformation<out TLift> where TLift : ICatalog<Guid>
    {
        TLift Lift { get; }
    }
}
