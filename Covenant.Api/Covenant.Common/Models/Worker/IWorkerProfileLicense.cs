using Covenant.Common.Entities;

namespace Covenant.Common.Models.Worker
{
    public interface IWorkerProfileLicense<out TFile> where TFile : ICovenantFile
    {
        TFile License { get; }
        string Number { get; }
        DateTime? Issued { get; }
        DateTime? Expires { get; }
    }
}
