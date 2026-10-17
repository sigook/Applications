namespace Covenant.Common.Models.Worker;

public interface IWorkerRequirementsInformation
{
    bool IsSubcontractor { get; }
    bool IsContractor { get; }
    bool IsUsa { get; }
    bool HasProfileImage { get; }
    bool HasTaxCategory { get; }
    string SocialInsurance { get; }
    bool SocialInsuranceExpire { get; }
    DateTime? DueDate { get; }
    bool HasSocialInsuranceFile { get; }
    string IdentificationNumber1 { get; }
    bool HasIdentification1File { get; }
    string IdentificationNumber2 { get; }
    bool HasIdentification2File { get; }
}
