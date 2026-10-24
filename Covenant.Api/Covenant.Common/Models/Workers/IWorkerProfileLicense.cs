using Covenant.Common.Entities;

namespace Covenant.Common.Models.Workers
{
    public interface IWorkerProfileLicense<out TFile> where TFile : ICovenantFile
    {
        TFile License { get; }
        string Number { get; }
        DateTime? Issued { get; }
        DateTime? Expires { get; }
    }
}
