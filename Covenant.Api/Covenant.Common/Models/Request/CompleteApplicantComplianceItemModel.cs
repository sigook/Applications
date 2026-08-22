namespace Covenant.Common.Models.Request;

public class CompleteApplicantComplianceItemModel
{
    public string FileName { get; set; }
    public string IdentificationNumber { get; set; }
    public Guid? IdentificationTypeId { get; set; }
    public string SocialInsuranceNumber { get; set; }
}
