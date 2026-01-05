namespace Covenant.Common.Models.Worker
{
    public interface IWorkerDocumentsInformation<out TIdentificationType, out TFile>
    {
        string IdentificationNumber1 { get; }
        TIdentificationType IdentificationType1 { get; }
        TFile IdentificationType1File { get; }
        string IdentificationNumber2 { get; }
        TIdentificationType IdentificationType2 { get; }
        TFile IdentificationType2File { get; }
        bool HavePoliceCheckBackground { get; }
        TFile PoliceCheckBackGround { get; }
        TFile Resume { get; }
    }
}
