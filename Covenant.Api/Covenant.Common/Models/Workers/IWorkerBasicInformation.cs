using Covenant.Common.Entities;

namespace Covenant.Common.Models.Worker
{
    public interface IWorkerBasicInformation<out TGender> where TGender : ICatalog<Guid>
    {
        string FirstName { get; }
        string MiddleName { get; }
        string LastName { get; }
        string SecondLastName { get; }
        DateTime BirthDay { get; }
        TGender Gender { get; }
        bool HasVehicle { get; }
    }
}
