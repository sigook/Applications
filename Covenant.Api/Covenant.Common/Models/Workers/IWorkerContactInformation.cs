using Covenant.Common.Entities;

namespace Covenant.Common.Models.Workers
{
    public interface IWorkerContactInformation<out TLocation, TCity>
        where TLocation : ILocation<TCity>
        where TCity : ICatalog<Guid>
    {
        string MobileNumber { get; }
        string Phone { get; }
        int? PhoneExt { get; }
        TLocation Location { get; }
    }
}
