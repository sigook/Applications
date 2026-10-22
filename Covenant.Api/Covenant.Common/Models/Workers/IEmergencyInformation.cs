namespace Covenant.Common.Models.Worker
{
    public interface IEmergencyInformation
    {
        bool HaveAnyHealthProblem { get; }
        string HealthProblem { get; }
        string ContactEmergencyPhone { get; }
        string OtherHealthProblem { get; }
        string ContactEmergencyName { get; }
        string ContactEmergencyLastName { get; }
    }
}
