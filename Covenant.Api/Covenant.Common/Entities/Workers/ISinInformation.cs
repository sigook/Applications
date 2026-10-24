namespace Covenant.Common.Entities.Workers
{
    public interface ISinInformation<out TFile> where TFile : ICovenantFile
    {
        string SocialInsurance { get; }
        bool SocialInsuranceExpire { get; }
        DateTime? DueDate { get; }
        TFile SocialInsuranceFile { get; }
    }
}